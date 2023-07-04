using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CustomerReviews.Data.Models;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CustomerReviews.Data.Repositories
{
    public class CustomerReviewRepository : DbContextRepositoryBase<CustomerReviewsDbContext>, ICustomerReviewRepository
    {
        public CustomerReviewRepository(CustomerReviewsDbContext dbContext)
            : base(dbContext)
        {
        }

        #region CustomerReviews

        public IQueryable<CustomerReviewEntity> CustomerReviews => DbContext.Set<CustomerReviewEntity>();

        public async Task<IList<CustomerReviewEntity>> GetByIdsAsync(IList<string> ids)
        {
            return await CustomerReviews.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task DeleteCustomerReviewsAsync(IList<string> ids)
        {
            var items = await GetByIdsAsync(ids);
            foreach (var item in items)
            {
                Remove(item);
            }
        }

        public async Task<IList<ReviewRatingCalculateDto>> GetCustomerReviewsByStoreProductAsync(string storeId, IList<string> entityIds, string entityType, IList<int> reviewStatuses)
        {
            var query = CustomerReviews
                .Where(r => r.StoreId == storeId && reviewStatuses.Contains(r.ReviewStatus));

            if (entityIds != null && entityIds.Any())
            {
                query = query.Where(r => entityIds.Contains(r.EntityId));
            }

            if (!string.IsNullOrEmpty(entityType))
            {
                query = query.Where(r => r.EntityType == entityType);
            }

            return await query
                .Select(r => new ReviewRatingCalculateDto
                {
                    StoreId = r.StoreId,
                    EntityId = r.EntityId,
                    EntityType = r.EntityType,
                    Rating = r.Rating,
                    CreatedDate = r.CreatedDate
                })
                .ToListAsync();
        }

        public async Task<IList<RequestReviewEntity>> GetReviewsWithEmptyAccessDate(DateTime maxModifiedDate, int maxRequests)
        {
            return await RequestReview
                .Where(r =>
                    r.AccessDate == null &&
                    r.ModifiedDate < maxModifiedDate &&
                    r.ReviewsRequest < maxRequests &&
                    !CustomerReviews.Any(cr =>
                        r.StoreId == cr.StoreId &&
                        r.EntityId == cr.EntityId &&
                        r.EntityType == "Product" &&
                        cr.UserId == r.UserId))
                .ToListAsync();
        }

        #endregion

        #region Rating

        public IQueryable<RatingEntity> Ratings => DbContext.Set<RatingEntity>();

        public Task<RatingEntity[]> GetAsync(IEnumerable<string> entityIds, string entityType)
        {
            return Ratings
                .Where(x => entityIds.Contains(x.EntityId) && x.EntityType == entityType)
                .ToArrayAsync();
        }

        public Task<RatingEntity[]> GetAsync(string storeId, IEnumerable<string> entityIds, string entityType)
        {
            return Ratings
                .Where(x => x.StoreId == storeId && entityIds.Contains(x.EntityId) && x.EntityType == entityType)
                .ToArrayAsync();
        }

        public Task<RatingEntity> GetAsync(string storeId, string entityId, string entityType)
        {
            return Ratings.FirstOrDefaultAsync(x => x.StoreId == storeId && x.EntityId == entityId && x.EntityType == entityType);
        }

        public void Delete(RatingEntity entity)
        {
            Remove(entity);
        }

        #endregion

        #region RequestReview

        public IQueryable<RequestReviewEntity> RequestReview => DbContext.Set<RequestReviewEntity>();


        public Task<RequestReviewEntity> GetRequestReviewByIdAsync(string id)
        {
            return RequestReview.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<RequestReviewEntity[]> GetRequestReviewByIdAsync(IEnumerable<string> ids)
        {
            return RequestReview.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        }

        public Task<RequestReviewEntity> GetRequestReviewAsync(string entityId, string entityType, string userId)
        {
            return RequestReview.FirstOrDefaultAsync(x => x.EntityId == entityId && x.EntityType == entityType && x.UserId == userId);
        }

        public void Delete(RequestReviewEntity entity)
        {
            Remove(entity);
        }

        #endregion
    }
}
