using VirtoCommerce.Domain.Commerce.Model.Search;

namespace VirtoCommerce.CustomerReviews.Core.Models
{
    public class CustomerReviewSearchCriteria : SearchCriteriaBase
    {
        public string[] ProductIds { get; set; }
        public CustomerReviewStatus? ReviewStatus { get; set; }
        public string StoreId { get; set; }
    }
}
