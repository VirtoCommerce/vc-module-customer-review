using VirtoCommerce.CustomerReviews.Core.Models;

namespace VirtoCommerce.CustomerReviews.Core.Events
{
    public class ReviewStatusChangeData
    {
        public string StoreId { get; set; }
        public string ProductId { get; set; }
        public CustomerReviewStatus OldStatus { get; set; }
        public CustomerReviewStatus NewStatus { get; set; }
    }
}
