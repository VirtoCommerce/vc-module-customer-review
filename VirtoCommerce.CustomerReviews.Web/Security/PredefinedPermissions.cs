namespace VirtoCommerce.CustomerReviews.Web.Security
{
    public static class PredefinedPermissions
    {
        public const string CustomerReviewRead = "customerReviews:read",
                    CustomerReviewUpdate = "customerReviews:update",
                    CustomerReviewDelete = "customerReviews:delete",
                    RatingRead = "customerReviews:ratingRead",
                    RatingRecalc = "customerReviews:ratingRecalc";
    }
}