namespace VirtoCommerce.CustomerReviews.Core.Models
{
    public class RatingStoreDto
    {
        public string StoreName { get; set; }
        public string StoreId { get; set; }
        public string ProductId { get; set; }
        public decimal Value { get; set; }
        public int ReviewCount { get; set; }
    }
}
