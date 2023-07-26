using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.CustomerReviews.Core.Services
{
    public interface ICustomerReviewService : ICrudService<CustomerReview>
    {
        Task ApproveReviewAsync(IList<string> customerReviewsIds);
        Task RejectReviewAsync(IList<string> customerReviewsIds);
        Task ResetReviewStatusAsync(IList<string> customerReviewsIds);
    }
}
