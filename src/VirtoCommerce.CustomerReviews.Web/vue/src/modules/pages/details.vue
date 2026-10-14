<template>
  <VcBlade
    :title="$t('CUSTOMER_REVIEWS.DETAILS.TITLE')"
    :toolbar-items="bladeToolbar"
    :loading="loading"
    width="50%"
  >
    <VcContainer>
      <VcForm class="reviews-form">
        <VcCard :header="$t('CUSTOMER_REVIEWS.FIELDS.ENTITY')">
          <div class="reviews-entity">
            <VcImage
              :src="entityImage"
              size="m"
              bordered
              :alt="item.entityName"
            />
            <div class="reviews-entity__info">
              <VcLink
                class="reviews-entity__name"
                @click="openEntity"
              >
                {{ item.entityName || item.entityId }}
              </VcLink>
              <VcRating
                :model-value="average"
                :max="5"
                variant="star-and-text"
                :label="$t('CUSTOMER_REVIEWS.DETAILS.AVERAGE')"
                placeholder="—"
              />
              <VcField
                v-if="item.entityType === 'Product'"
                :label="$t('CUSTOMER_REVIEWS.FIELDS.STORE')"
                :model-value="item.storeName || '—'"
              />
            </div>
          </div>
        </VcCard>

        <VcCard :header="$t('CUSTOMER_REVIEWS.DETAILS.TITLE')">
          <div class="reviews-card">
            <VcRow class="tw-gap-4">
              <VcCol>
                <VcField
                  :label="$t('CUSTOMER_REVIEWS.FIELDS.CREATED_BY')"
                  :model-value="item.userName || '—'"
                />
              </VcCol>
              <VcCol>
                <VcField
                  :label="$t('CUSTOMER_REVIEWS.FIELDS.CREATED_DATE')"
                  :model-value="item.createdDate"
                  type="date"
                />
              </VcCol>
            </VcRow>
            <VcField
              :label="$t('CUSTOMER_REVIEWS.FIELDS.TITLE')"
              :model-value="item.title || '—'"
            />
            <VcRow class="tw-gap-4">
              <VcCol>
                <VcRating
                  :model-value="item.rating"
                  :max="5"
                  :label="$t('CUSTOMER_REVIEWS.FIELDS.RATING')"
                />
              </VcCol>
              <VcCol>
                <div class="reviews-field">
                  <VcLabel>{{ $t("CUSTOMER_REVIEWS.FIELDS.STATUS") }}</VcLabel>
                  <div class="tw-inline-flex">
                    <VcStatus :variant="statusVariant(item.reviewStatus)">{{ $t(statusKey(item.reviewStatus)) }}</VcStatus>
                  </div>
                </div>
              </VcCol>
            </VcRow>
            <VcField
              class="reviews-text"
              :label="$t('CUSTOMER_REVIEWS.FIELDS.REVIEW')"
              :model-value="item.review || '—'"
            />
          </div>
        </VcCard>

        <!-- legacy review-image-widget (the module's own reviewDetail group): the review's images, preview only -->
        <VcCard :header="$t('CUSTOMER_REVIEWS.DETAILS.IMAGES')">
          <div class="reviews-card">
            <VcGallery
              :images="images"
              disabled
              size="sm"
              imagefit="cover"
              :item-actions="{ preview: true, edit: false, remove: false }"
            >
              <template #empty>
                <div class="reviews-empty">{{ $t("CUSTOMER_REVIEWS.DETAILS.NO_IMAGES") }}</div>
              </template>
            </VcGallery>
          </div>
        </VcCard>
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script setup lang="ts">
// review-detail: a customer's review (read-only) with moderation commands; the list passes the row (there is no
// get-by-id endpoint), so a page reload doesn't restore the blade
import { notification, useApiClient, useAsync, useBlade, usePopup, type IBladeToolbar } from "@vc-shell/framework";
import {
  VcBlade,
  VcCard,
  VcCol,
  VcContainer,
  VcField,
  VcForm,
  VcGallery,
  VcImage,
  VcLabel,
  VcLink,
  VcRating,
  VcRow,
  VcStatus,
} from "@vc-shell/framework/ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import {
  CustomerReviewsModuleClient,
  CustomerReviewsModuleRatingClient,
  type CustomerReviewListItem,
} from "../../api_client/virtocommerce.customerreviews";
import { getJson, statusKey, statusVariant } from "../utils";

defineBlade({
  url: "/customer-review",
  name: "CustomerReviewDetails",
  permissions: ["customerReviews:update"],
  routable: false,
});

const { t } = useI18n({ useScope: "global" });
const { options, openBlade, callParent, closeSelf } = useBlade<{ item?: CustomerReviewListItem }>();
const { showConfirmation } = usePopup();
const { getApiClient } = useApiClient(CustomerReviewsModuleClient);
const { getApiClient: getRatingClient } = useApiClient(CustomerReviewsModuleRatingClient);

const item = ref<CustomerReviewListItem>(options.value?.item ?? {});
const average = ref<number>();
const entityImage = ref<string>();
const images = computed(() => [...(item.value.images ?? [])].sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0)));
const isNew = computed(() => item.value.reviewStatusId === 0);

onMounted(async () => {
  const { entityId, entityType } = item.value;
  if (!entityId) return;
  // legacy: the entity's average rating ("4.5/5") and, for products, its name and image
  const [[rating], product] = await Promise.all([
    (await getRatingClient()).getEntityRating({ entityIds: [entityId], entityType }).catch(() => []),
    entityType === "Product"
      ? getJson<{ imgSrc?: string }>(`/api/catalog/products/${entityId}?respGroup=ItemInfo,ItemAssets`).catch(() => undefined)
      : undefined,
  ]);
  average.value = rating?.value ? Number(rating.value.toFixed(1)) : undefined;
  entityImage.value = product?.imgSrc ?? undefined;
});

function openEntity() {
  if (item.value.entityType === "Product") openBlade({ name: "CatalogProductDetails", param: item.value.entityId });
  else notification.error(t("CUSTOMER_REVIEWS.ALERTS.UNKNOWN_ENTITY_TYPE", { type: item.value.entityType }));
}

// legacy: every moderation command refreshes the list and closes the review
const { loading, action: run } = useAsync(async (change?: (ids: string[]) => Promise<void>) => {
  await change!([item.value.id!]);
  callParent("reload");
  await closeSelf();
});

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    id: "approve",
    title: t("CUSTOMER_REVIEWS.TOOLBAR.APPROVE"),
    icon: "lucide-check",
    permissions: "customerReviews:update",
    disabled: computed(() => !isNew.value),
    clickHandler: async () => run(async (ids) => (await getApiClient()).approveReview(ids)),
  },
  {
    id: "reject",
    title: t("CUSTOMER_REVIEWS.TOOLBAR.REJECT"),
    icon: "lucide-ban",
    permissions: "customerReviews:update",
    disabled: computed(() => !isNew.value),
    clickHandler: async () => run(async (ids) => (await getApiClient()).rejectReview(ids)),
  },
  {
    id: "reset",
    title: t("CUSTOMER_REVIEWS.TOOLBAR.RESET"),
    icon: "lucide-rotate-ccw",
    permissions: "customerReviews:update",
    disabled: computed(() => isNew.value),
    clickHandler: async () => run(async (ids) => (await getApiClient()).resetReviewStatus(ids)),
  },
  {
    id: "delete",
    title: t("CUSTOMER_REVIEWS.TOOLBAR.DELETE"),
    icon: "lucide-trash-2",
    permissions: "customerReviews:delete",
    async clickHandler() {
      if (await showConfirmation(t("CUSTOMER_REVIEWS.ALERTS.DELETE")))
        await run(async (ids) => (await getApiClient()).delete(ids));
    },
  },
]);

</script>

<style lang="scss">
.reviews-form {
  padding: 20px 22px 36px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.reviews-card {
  display: flex;
  flex-direction: column;
  gap: 16px;
  padding: 16px;
}

.reviews-entity {
  display: flex;
  align-items: flex-start;
  gap: 16px;
  padding: 16px;

  &__info {
    display: flex;
    flex-direction: column;
    gap: 12px;
    min-width: 0;
  }

  &__name {
    font-size: 16px;
    font-weight: 600;
  }
}

.reviews-field {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 4px;
}

.reviews-text .vc-field__text {
  white-space: pre-wrap;
}

.reviews-empty {
  color: var(--neutrals-500);
  font-size: 14px;
}
</style>
