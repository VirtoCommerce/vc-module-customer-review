using System;
using VirtoCommerce.Domain.Commerce.Model.Search;

namespace VirtoCommerce.CustomerReviews.Core.Models
{
    public class CustomerReviewSearchCriteria : SearchCriteriaBase
    {
        public string[] ProductIds { get; set; }
        public int[] ReviewStatus { get; set; }
        public string StoreId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string UserId { get; set; }
    }
}
