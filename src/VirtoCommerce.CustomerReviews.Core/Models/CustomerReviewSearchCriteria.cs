using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.Core.Models
{
    public class CustomerReviewSearchCriteria : SearchCriteriaBase
    {
        public string[] EntityIds { get; set; }

        public string EntityType { get; set; }

        public CustomerReviewStatus[] ReviewStatus { get; set; }

        public string StoreId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? StartRating { get; set; }

        public int? EndRating { get; set; }

        public string UserId { get; set; }
    }
}
