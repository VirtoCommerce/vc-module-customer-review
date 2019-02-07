using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Models;

namespace VirtoCommerce.CustomerReviews.Core.Services
{
    public interface ICustomerReviewService
    {
        Task<IEnumerable<CustomerReview>> GetByIdsAsync(IEnumerable<string> customerReviewsIds);
        Task SaveCustomerReviewsAsync(IEnumerable<CustomerReview> items);

        Task ApproveReviewAsync(IEnumerable<string> customerReviewsIds);
        Task RejectReviewAsync(IEnumerable<string> customerReviewsIds);
        Task ResetReviewStatusAsync(IEnumerable<string> customerReviewsIds);

        Task DeleteCustomerReviewsAsync(IEnumerable<string> ids);

    }
}
