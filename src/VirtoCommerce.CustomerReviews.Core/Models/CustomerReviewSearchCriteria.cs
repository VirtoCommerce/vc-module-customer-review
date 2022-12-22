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

        [Obsolete("Use StartDate and EndDate instead")]
        public DateTime? ModifiedDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? StartRating { get; set; }

        public int? EndRating { get; set; }

        public string UserId { get; set; }
    }
}
