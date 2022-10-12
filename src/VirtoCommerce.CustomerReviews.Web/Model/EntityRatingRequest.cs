namespace VirtoCommerce.CustomerReviews.Web.Model
{
    public class EntityRatingRequest
    {
        public string[] EntityIds { get; set; }
        public string EntityType { get; set; }
        public string CatalogId { get; set; }
    }
}
