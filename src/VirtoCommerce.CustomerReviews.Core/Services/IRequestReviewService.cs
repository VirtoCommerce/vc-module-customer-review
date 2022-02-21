using System.Threading.Tasks;

namespace VirtoCommerce.CustomerReviews.Core.Services
{
    public interface IRequestReviewService
    {
        Task MarkAccessRequest(string[] requestIds);
    }
}
