namespace VirtoCommerce.CustomerReviews.Core.Models
{
    public class RatingEntityStoreDto
    {
        public string StoreName { get; set; }
        public string StoreId { get; set; }
        public string EntityId { get; set; }
        public string EntityType { get; set; }
        public decimal Value { get; set; }
        public int ReviewCount { get; set; }
    }
}
