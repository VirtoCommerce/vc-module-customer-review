using System;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Models;

namespace VirtoCommerce.CustomerReviews.Core.Services
{
    public interface IRatingService
    {
        Task CalculateAsync(ReviewStatusChangeData[] data);
        Task CalculateAsync(string storeId);

        [Obsolete("Use generic entityRating method")]
        Task<RatingProductDto[]> GetForStoreAsync(string storeId, string[] productIds);
        [Obsolete("Use generic entityRating method")]
        Task<RatingStoreDto[]> GetForCatalogAsync(string catalogId, string[] productIds);

        Task<RatingEntityDto[]> GetForStoreAsync(string storeId, string[] entityIds, string entityType);
        Task<RatingEntityStoreDto[]> GetRatingsAsync(string[] entityIds, string entityType);
    }
}
