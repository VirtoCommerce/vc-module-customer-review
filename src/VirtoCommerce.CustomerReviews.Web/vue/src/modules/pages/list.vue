<template>
  <VcBlade
    :title="title"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="50%"
  >
    <VcDataTable
      v-model:search-value="searchValue"
      v-model:sort-field="sortField"
      v-model:sort-order="sortOrder"
      :items="items"
      :total-count="totalCount"
      :pagination="pagination"
      :searchable="true"
      :search-placeholder="$t('CUSTOMER_REVIEWS.LIST.SEARCH')"
      :state-key="stateKey"
      :active-item-id="activeChildParam"
      :show-all-columns="!activeChildParam && !entityFilter"
      :global-filters="globalFilters"
      :empty-state="{ icon: 'lucide-messages-square', title: $t('CUSTOMER_REVIEWS.LIST.EMPTY') }"
      @row-click="(e: { data: CustomerReviewListItem }) => openDetails(e.data)"
      @pagination-click="pagination.goToPage"
      @filter="onFilter"
    >
      <VcColumn
        id="title"
        :title="$t('CUSTOMER_REVIEWS.FIELDS.TITLE')"
        mobile-role="title"
        sortable
        always-visible
      />
      <VcColumn
        v-if="!entityFilter"
        id="entityName"
        :title="$t('CUSTOMER_REVIEWS.FIELDS.ENTITY')"
        mobile-role="field"
        sortable
      />
      <VcColumn
        id="rating"
        :title="$t('CUSTOMER_REVIEWS.FIELDS.RATING')"
        mobile-role="field"
        sortable
        always-visible
      >
        <template #body="{ data }">
          <VcRating
            :model-value="data.rating"
            :max="5"
          />
        </template>
      </VcColumn>
      <VcColumn
        id="createdDate"
        :title="$t('CUSTOMER_REVIEWS.FIELDS.CREATED_DATE')"
        type="date"
        mobile-role="field"
        sortable
      />
      <VcColumn
        id="reviewStatus"
        :title="$t('CUSTOMER_REVIEWS.FIELDS.STATUS')"
        mobile-role="status"
        sortable
        always-visible
      >
        <template #body="{ data }">
          <div class="dash-inline">
            <VcStatus :variant="statusVariant(data.reviewStatus)">{{ $t(statusKey(data.reviewStatus)) }}</VcStatus>
          </div>
        </template>
      </VcColumn>
      <VcColumn
        id="userName"
        :title="$t('CUSTOMER_REVIEWS.FIELDS.CREATED_BY')"
        mobile-role="field"
        sortable
      />
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
// reviews-list: the reviews of all entities (menu) or of one product (opened by the product rating widget)
import {
  useApiClient,
  useAsync,
  useBlade,
  useDataTablePagination,
  useDataTableSort,
  useFunctions,
  useLatestRequest,
  usePermissions,
  useTableSearch,
  type IBladeToolbar,
} from "@vc-shell/framework";
import { VcBlade, VcColumn, VcDataTable, VcRating, VcStatus } from "@vc-shell/framework/ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import {
  CustomerReviewsModuleClient,
  CustomerReviewStatus,
  type CustomerReviewListItem,
} from "../../api_client/virtocommerce.customerreviews";
import { ENTITY_TYPES, getJson, statusKey, statusVariant } from "../utils";

defineBlade({
  url: "/customer-reviews",
  name: "CustomerReviewsList",
  isWorkspace: true,
  permissions: ["customerReviews:read"],
  menuItem: {
    title: "CUSTOMER_REVIEWS.MENU.TITLE",
    icon: "lucide-messages-square",
    priority: 50,
  },
});

const { t } = useI18n({ useScope: "global" });
const { param, options, activeChildParam, openBlade, exposeToChildren } = useBlade<{ entityName?: string }>();
const { hasAccess } = usePermissions();
const { debounce } = useFunctions();
const { getApiClient } = useApiClient(CustomerReviewsModuleClient);

// opened from a product (param: product id): its reviews only (legacy filter { entityIds, entityType })
const entityName = ref(options.value?.entityName);
const entityFilter = computed(() =>
  param.value ? { entityIds: [param.value], entityType: "Product", entityName: entityName.value } : undefined,
);
// a page reload restores the param but no options: read the product's name back
if (param.value && !entityName.value)
  getJson<{ name?: string }>(`/api/catalog/products/${encodeURIComponent(param.value)}?respGroup=ItemInfo`)
    .then((p) => (entityName.value = p.name))
    .catch(() => undefined);
const stateKey = computed(() => (entityFilter.value ? "CUSTOMER_REVIEWS_ENTITY" : "CUSTOMER_REVIEWS"));
const title = computed(() =>
  entityFilter.value
    ? t("CUSTOMER_REVIEWS.LIST.TITLE_ENTITY", { name: entityFilter.value.entityName ?? "" })
    : t("CUSTOMER_REVIEWS.MENU.TITLE"),
);

const PAGE_SIZE = 20;
const items = ref<CustomerReviewListItem[]>([]);
const totalCount = ref(0);
const filters = ref<{ reviewStatus?: CustomerReviewStatus[]; entityType?: string }>({});

const { sortField, sortOrder, sortExpression } = useDataTableSort({
  stateKey: stateKey.value,
  initialField: "createdDate",
  initialDirection: "DESC",
});
const { searchValue } = useTableSearch({ stateKey: stateKey.value });
const pagination = useDataTablePagination({ stateKey: stateKey.value, pageSize: PAGE_SIZE, totalCount });
const search = useLatestRequest();

const { loading, action: load } = useAsync(async () => {
  const result = await search.latest(
    (await getApiClient()).getCustomerReviewsList({
      // the API matches the phrase in the review text
      searchPhrase: searchValue.value || undefined,
      sort: sortExpression.value,
      skip: pagination.skip,
      take: PAGE_SIZE,
      ...filters.value,
      entityIds: entityFilter.value?.entityIds,
      entityType: entityFilter.value?.entityType ?? filters.value.entityType,
    }),
  );
  if (!result) return;
  items.value = result.results ?? [];
  totalCount.value = result.totalCount ?? 0;
});

const globalFilters = computed(() => [
  {
    id: "reviewStatus",
    label: t("CUSTOMER_REVIEWS.FIELDS.STATUS"),
    filter: { options: Object.values(CustomerReviewStatus).map((s) => ({ value: s, label: t(statusKey(s)) })) },
  },
  ...(entityFilter.value
    ? []
    : [
        {
          id: "entityType",
          label: t("CUSTOMER_REVIEWS.FIELDS.ENTITY_TYPE"),
          filter: { options: ENTITY_TYPES.map((x) => ({ value: x, label: x })) },
        },
      ]),
]);

function onFilter(event: { filters: Record<string, unknown> }) {
  const status = event.filters.reviewStatus as CustomerReviewStatus | CustomerReviewStatus[] | undefined;
  const entityType = event.filters.entityType as string | string[] | undefined;
  filters.value = {
    reviewStatus: status ? ([] as CustomerReviewStatus[]).concat(status) : undefined,
    entityType: Array.isArray(entityType) ? entityType[0] : entityType || undefined,
  };
  // a page change loads through the watcher below
  if (pagination.skip) pagination.setPage(1);
  else load();
}

load();
watch(searchValue, () => pagination.setPage(1));
watch([sortExpression, searchValue, () => pagination.skip], debounce(load, 300));

const bladeToolbar = ref<IBladeToolbar[]>([
  { id: "refresh", title: t("CUSTOMER_REVIEWS.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: () => load() },
]);

// legacy: a review opens only for users who may moderate it
function openDetails(item: CustomerReviewListItem) {
  if (!hasAccess("customerReviews:update")) return;
  openBlade({ name: "CustomerReviewDetails", param: item.id, options: { item } });
}

exposeToChildren({ reload: () => load() });
</script>
