using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Models;

namespace VirtoCommerce.CustomerReviews.Core.Services
{
    public interface ICustomerReviewService
    {
        Task ApproveReviewAsync(string[] customerReviewsIds);
        Task RejectReviewAsync(string[] customerReviewsIds);
        Task ResetReviewStatusAsync(string[] customerReviewsIds);
    }
}
