import path from "node:path";
import { getDynamicModuleConfiguration } from "@vc-shell/mf-module";

// Rating and reviews admin UI for the Admin Shell host (Module Federation remote),
// served by the platform at /modules/$(VirtoCommerce.CustomerReviews)/plugins/admin-shell/
const base = "/modules/$(VirtoCommerce.CustomerReviews)/plugins/admin-shell/";
process.env.APP_BASE_PATH = base;

export default getDynamicModuleConfiguration({
  appId: "admin-shell",
  moduleRoot: path.resolve(__dirname, ".."),
  entry: "./src/modules/index.ts",
  remoteName: "customerreviews_admin",
  base,
});
