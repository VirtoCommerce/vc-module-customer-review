using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.CustomerReviews.Core.Services
{
    public interface ICustomerReviewSearchService : ISearchService<CustomerReviewSearchCriteria, CustomerReviewSearchResult, CustomerReview>
    {
        Task<string[]> GetProductIdsOfModifiedReviewsAsync(ChangedReviewsQuery criteria);
    }
}
