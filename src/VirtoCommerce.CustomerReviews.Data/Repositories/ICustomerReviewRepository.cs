using System;
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

        Task<IList<CustomerReviewEntity>> GetByIdsAsync(IList<string> ids);
        Task DeleteCustomerReviewsAsync(IList<string> ids);

        Task<IList<ReviewRatingCalculateDto>> GetCustomerReviewsByStoreProductAsync(string storeId, IList<string> entityIds, string entityType, IList<int> reviewStatuses);
        Task<IList<RequestReviewEntity>> GetReviewsWithEmptyAccessDate(DateTime maxModifiedDate, int maxRequests);

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
        Task<RequestReviewEntity> GetRequestReviewByIdAsync(string id);
        Task<RequestReviewEntity[]> GetRequestReviewByIdAsync(IEnumerable<string> ids);
        Task<RequestReviewEntity> GetRequestReviewAsync(string entityId, string entityType, string userId);
        void Delete(RequestReviewEntity entity);

        #endregion
    }
}
