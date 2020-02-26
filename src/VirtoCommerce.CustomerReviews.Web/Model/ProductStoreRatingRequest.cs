namespace VirtoCommerce.CustomerReviews.Web.Model
{
    public class ProductStoreRatingRequest
    {
        public string StoreId { get; set; }
        public string[] ProductIds { get; set; }
    }
}