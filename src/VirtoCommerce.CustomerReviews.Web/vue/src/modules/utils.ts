// Entity types the reviews can be about (legacy entityTypesResolverService registered only "Product")
export const ENTITY_TYPES = ["Product"];

// VcStatus variant of a review status (CustomerReviewStatus: New, Approved, Rejected)
export function statusVariant(status?: string): "success" | "warning" | "danger" | "info" {
  return status === "Approved" ? "success" : status === "Rejected" ? "danger" : status === "New" ? "warning" : "info";
}

// locale key of a review status label
export const statusKey = (status?: string) => `CUSTOMER_REVIEWS.STATUS.${status}`;

// Same-origin JSON GET for other modules' endpoints (catalog products); cookie auth works because the host is same-origin
export async function getJson<T>(url: string): Promise<T> {
  const res = await fetch(url, { headers: { Accept: "application/json" } });
  if (!res.ok) throw new Error(`GET ${url}: ${res.status}`);
  return (await res.json()) as T;
}
