using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Data.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.Data.Repositories
{
    public interface ICustomerReviewRepository : IRepository
    {
        #region CustomerReviews
        IQueryable<CustomerReviewEntity> CustomerReviews { get; }

        Task<IEnumerable<CustomerReviewEntity>> GetByIdsAsync(IEnumerable<string> ids);
        Task DeleteCustomerReviewsAsync(IEnumerable<string> ids);

        Task<ReviewRatingCalculateDto[]> GetCustomerReviewsByStoreProductAsync(string storeId, IEnumerable<string> productIds, IEnumerable<int> reviewStatuses);

        #endregion

        #region Rating

        IQueryable<RatingEntity> Ratings { get; }
        Task<RatingEntity> GetAsync(string storeId, string productId);
        Task<RatingEntity[]> GetAsync(string storeId, IEnumerable<string> productIds);
        void Delete(RatingEntity entity);

        #endregion
    }
}
