using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.Core.Models
{
    public class CustomerReviewSearchCriteria : SearchCriteriaBase
    {
        public string[] EntityIds { get; set; }
        public string EntityType { get; set; }
        public int[] ReviewStatus { get; set; }
        public string StoreId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string UserId { get; set; }
    }
}
