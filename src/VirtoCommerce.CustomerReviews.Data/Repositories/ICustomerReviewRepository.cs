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

        Task<ReviewRatingCalculateDto[]> GetCustomerReviewsByStoreProductAsync(string storeId, IEnumerable<string> entityIds, string entityType, IEnumerable<int> reviewStatuses);

        #endregion

        #region Rating

        IQueryable<RatingEntity> Ratings { get; }
        Task<RatingEntity[]> GetAsync(IEnumerable<string> entityIds, string entityType);
        Task<RatingEntity> GetAsync(string storeId, string entityId, string entityType);
        Task<RatingEntity[]> GetAsync(string storeId, IEnumerable<string> entityIds, string entityType);
        void Delete(RatingEntity entity);

        #endregion

        #region RequestReview

        IQueryable<RequestReviewEntity> RequestReview { get; }
        Task<RequestReviewEntity> GetRequestReviewByIdAsync(string Id);
        Task<RequestReviewEntity[]> GetRequestReviewByIdAsync(IEnumerable<string> Ids);
        Task<RequestReviewEntity> GetRequestReviewAsync(string entityId, string entityType, string userId);
        void Delete(RequestReviewEntity entity);

        #endregion

    }
}
