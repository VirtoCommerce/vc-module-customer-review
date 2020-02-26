using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Models;

namespace VirtoCommerce.CustomerReviews.Core.Services
{
    public interface ICustomerReviewService
    {
        Task<CustomerReview[]> GetByIdsAsync(string[] customerReviewsIds);
        Task SaveCustomerReviewsAsync(CustomerReview[] items);

        Task ApproveReviewAsync(string[] customerReviewsIds);
        Task RejectReviewAsync(string[] customerReviewsIds);
        Task ResetReviewStatusAsync(string[] customerReviewsIds);

        Task DeleteCustomerReviewsAsync(string[] ids);

    }
}
