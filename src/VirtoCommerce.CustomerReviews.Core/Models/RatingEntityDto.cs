namespace VirtoCommerce.CustomerReviews.Core.Models
{
    public class RatingEntityDto
    {
        public string EntityId { get; set; }
        public string EntityType { get; set; }
        public decimal Value { get; set; }
        public int ReviewCount { get; set; }
    }
}
