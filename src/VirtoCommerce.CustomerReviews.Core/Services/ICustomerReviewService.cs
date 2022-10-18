using System.Threading.Tasks;

namespace VirtoCommerce.CustomerReviews.Core.Services
{
    public interface ICustomerReviewService
    {
        Task ApproveReviewAsync(string[] customerReviewsIds);
        Task RejectReviewAsync(string[] customerReviewsIds);
        Task ResetReviewStatusAsync(string[] customerReviewsIds);
        Task DeleteReviews(string[] customerReviewsIds);
    }
}
