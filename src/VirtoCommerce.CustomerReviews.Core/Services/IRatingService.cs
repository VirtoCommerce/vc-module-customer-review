using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Models;

namespace VirtoCommerce.CustomerReviews.Core.Services
{
    public interface IRatingService
    {
        Task CalculateAsync(ReviewStatusChangeData[] data);
        Task CalculateAsync(string storeId);

        Task<RatingEntityDto[]> GetForStoreAsync(string storeId, string[] entityIds, string entityType);

        Task<RatingEntityStoreDto[]> GetRatingsAsync(string[] entityIds, string entityType);
    }
}
