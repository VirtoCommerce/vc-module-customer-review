// The count on this module's menu item ("Ratings & Reviews"): reviews to moderate. Registered with the Admin Shell's menu counter
// registry (app.config.globalProperties.$menuCounters; see the shell's docs/menu-counters.md), which asks it once the
// menu arrives and then every minute while the tab is visible.
import type { App } from "vue";

/** The part of the shell's menu counter registry this module uses */
interface MenuCounters {
  register(counter: {
    itemId: string;
    hint: string;
    permission?: string | string[];
    count(context: { signal: AbortSignal }): Promise<number>;
  }): void;
}

export function registerReviewsCounter(app: App) {
  const counters = app.config.globalProperties.$menuCounters as MenuCounters | undefined;
  // a vc-shell host without menu counters
  if (!counters) return;
  counters.register({
    itemId: "customerReviews",
    hint: "CUSTOMER_REVIEWS.MENU.COUNTER",
    permission: "customerReviews:read",
    // take 0: only the total travels
    count: async ({ signal }) => {
      const response = await fetch("/api/customerReviews/search", {
        method: "POST",
        headers: { Accept: "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({ reviewStatus: ["New"], take: 0 }),
        signal,
      });
      if (!response.ok) throw new Error(`${response.status} ${response.statusText}: /api/customerReviews/search`);
      const result = (await response.json()) as { totalCount?: number };
      return result.totalCount ?? 0;
    },
  });
}
