using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.Core.Models
{
    public class ReviewStatusChangeData : IEntity
    {
        public string Id { get; set; }
        public string StoreId { get; set; }
        public string EntityId { get; set; }
        public string EntityType { get; set; }
        public CustomerReviewStatus OldStatus { get; set; }
        public CustomerReviewStatus NewStatus { get; set; }
    }
}
