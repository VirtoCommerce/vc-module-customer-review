using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Domain.Commerce.Model.Search;

namespace VirtoCommerce.CustomerReviews.Core.Services
{
    public interface ICustomerReviewSearchService
    {
        Task<GenericSearchResult<CustomerReview>> SearchCustomerReviewsAsync(CustomerReviewSearchCriteria criteria);
    }
}
