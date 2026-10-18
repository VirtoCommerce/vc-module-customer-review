import * as blades from "./pages";
import * as locales from "./locales";
import { defineAppModule, registerExternalWidget, usePermissions } from "@vc-shell/framework";
import { markRaw, type App } from "vue";
import ProductRatingWidget from "./widgets/ProductRatingWidget.vue";
import { registerReviewsCounter } from "./menuCounter";

// The product's rating and reviews on the Catalog module's product blade (legacy itemDetail widget, customerReviews:read)
registerExternalWidget({
  id: "CustomerReviewsProductRating",
  title: "Reviews",
  component: markRaw(ProductRatingWidget),
  targetBlades: ["CatalogProductDetails"],
  isVisible: (blade) => !!blade?.param && usePermissions().hasAccess("customerReviews:read"),
});

const appModule = defineAppModule({
  blades,
  locales,
});

export default {
  install(app: App) {
    appModule.install(app);
    // reviews to moderate on the menu item
    registerReviewsCounter(app);
  },
};
