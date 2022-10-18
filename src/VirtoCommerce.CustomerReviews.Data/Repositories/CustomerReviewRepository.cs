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
        public CustomerReviewRepository(CustomerReviewsDbContext dbContext) : base(dbContext)
        {
        }

        #region CustomerReviews
        public IQueryable<CustomerReviewEntity> CustomerReviews => DbContext.Set<CustomerReviewEntity>();

        public Task<IEnumerable<CustomerReviewEntity>> GetByIdsAsync(IEnumerable<string> ids)
        {
            return Task.FromResult<IEnumerable<CustomerReviewEntity>>(CustomerReviews.Where(x => ids.Contains(x.Id)).ToList());
        }

        public async Task DeleteCustomerReviewsAsync(IEnumerable<string> ids)
        {
            var items = await GetByIdsAsync(ids);
            foreach (var item in items)
            {
                Remove(item);
            }
        }

        public Task<ReviewRatingCalculateDto[]> GetCustomerReviewsByStoreProductAsync(string storeId, IEnumerable<string> entityIds, string entityType, IEnumerable<int> reviewStatuses)
        {
            var q = CustomerReviews
                   .Where(r => r.StoreId == storeId && reviewStatuses.Contains(r.ReviewStatus));

            if (entityIds != null && entityIds.Any())
            {
                q = q.Where(r => entityIds.Contains(r.EntityId));
            }

            if (!string.IsNullOrEmpty(entityType))
            {
                q = q.Where(r => r.EntityType == entityType);
            }

            return q.Select(r => new ReviewRatingCalculateDto()
            {
                StoreId = r.StoreId,
                EntityId = r.EntityId,
                EntityType = r.EntityType,
                Rating = r.Rating,
                CreatedDate = r.CreatedDate
            })
            .ToArrayAsync();

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


        public Task<RequestReviewEntity> GetRequestReviewByIdAsync(string Id)
        {
            return RequestReview.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public Task<RequestReviewEntity[]> GetRequestReviewByIdAsync(IEnumerable<string> Ids)
        {
            return RequestReview.Where(x => Ids.Contains(x.Id)).ToArrayAsync();
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
