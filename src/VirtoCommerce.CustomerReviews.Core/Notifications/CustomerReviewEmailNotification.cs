using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.CustomerReviews.Core.Notifications
{
    public class CustomerReviewEmailNotification : EmailNotification
    {
        public CustomerReviewEmailNotification () : base(nameof(CustomerReviewEmailNotification))
        {

        }

        public string RequestId { get; set; }
        public virtual Member Customer { get; set; }
        public virtual LineItem Item { get; set; }
    }
}
