import * as blades from "./pages";
import * as locales from "./locales";
import { defineAppModule, registerExternalWidget, usePermissions } from "@vc-shell/framework";
import { markRaw } from "vue";
import ProductRatingWidget from "./widgets/ProductRatingWidget.vue";

// The product's rating and reviews on the Catalog module's product blade (legacy itemDetail widget, customerReviews:read)
registerExternalWidget({
  id: "CustomerReviewsProductRating",
  title: "Reviews",
  component: markRaw(ProductRatingWidget),
  targetBlades: ["CatalogProductDetails"],
  isVisible: (blade) => !!blade?.param && usePermissions().hasAccess("customerReviews:read"),
});

export default defineAppModule({
  blades,
  locales,
});
