using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.Core.Models
{
    public class CustomerReview : AuditableEntity, ICloneable
    {
        public string Title { get; set; }
        public string Review { get; set; }
        public int Rating { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ProductId { get; set; }
        public string StoreId { get; set; }
        public CustomerReviewStatus ReviewStatus { get; set; }

        public object Clone()
        {
            return MemberwiseClone() as CustomerReview;
        }
    }
}
