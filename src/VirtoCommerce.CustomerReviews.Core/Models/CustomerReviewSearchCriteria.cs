using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.Core.Models
{
    public class CustomerReviewSearchCriteria : SearchCriteriaBase
    {
        public string[] EntityIds { get; set; }
        public string EntityType { get; set; }

        [Obsolete("Use EntityIds and EntityType instead")]
        public string[] ProductIds
        {
            get { return EntityIds; }
            set { EntityIds = value; EntityType = "Product"; }
        }

        public int[] ReviewStatus { get; set; }
        public string StoreId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string UserId { get; set; }
    }
}
