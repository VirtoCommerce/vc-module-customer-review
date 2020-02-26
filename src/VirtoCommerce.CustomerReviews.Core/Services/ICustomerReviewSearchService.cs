using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.Core.Services
{
    public interface ICustomerReviewSearchService
    {
        Task<GenericSearchResult<CustomerReview>> SearchCustomerReviewsAsync(CustomerReviewSearchCriteria criteria);
        Task<string[]> GetProductIdsOfModifiedReviews(ChangedReviewsQuery criteria);
    }
}
