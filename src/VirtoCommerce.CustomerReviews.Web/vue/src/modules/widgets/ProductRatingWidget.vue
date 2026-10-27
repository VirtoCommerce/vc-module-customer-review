<template>
  <VcWidget
    icon="lucide-star"
    :title="rating ? $t('CUSTOMER_REVIEWS.WIDGETS.RATING', { rating: rating.value!.toFixed(1) }) : $t('CUSTOMER_REVIEWS.WIDGETS.NO_RATING')"
    :value="canReadRating ? (rating?.reviewCount ?? 0) : undefined"
    :disabled="!productId"
    @click="open"
  />
</template>

<script setup lang="ts">
// product-rating-widget on the Catalog module's product blade (itemDetail): the product's rating and review count;
// opens the product's reviews. The Catalog plugin shares the product via defineBladeContext({ item })
import { computed, ref, watch } from "vue";
import { injectBladeContext, useApiClient, useBlade, usePermissions, useWidgetTrigger } from "@vc-shell/framework";
import { VcWidget } from "@vc-shell/framework/ui";
import { CustomerReviewsModuleRatingClient, type RatingEntityStoreDto } from "../../api_client/virtocommerce.customerreviews";

const ctx = injectBladeContext();
const product = computed(() => ctx.value.item as { id?: string; name?: string } | undefined);
const productId = computed(() => product.value?.id);
const { openBlade } = useBlade();
const { getApiClient } = useApiClient(CustomerReviewsModuleRatingClient);
// the rating API needs ratingRead; without it the widget only opens the reviews, with no rating or count
const canReadRating = usePermissions().hasAccess("customerReviews:ratingRead");
// legacy shows the first store's rating
const rating = ref<RatingEntityStoreDto>();

async function load() {
  rating.value =
    productId.value && canReadRating
      ? (await (await getApiClient()).getEntityRating({ entityIds: [productId.value], entityType: "Product" }).catch(() => []))[0]
      : undefined;
}
watch(productId, load, { immediate: true });
useWidgetTrigger({ onRefresh: load });

// the product id is the param, so a page reload keeps the list filtered to this product
const open = () =>
  openBlade({
    name: "CustomerReviewsList",
    param: productId.value,
    options: { entityName: product.value?.name },
    onClose: load,
  });
</script>
