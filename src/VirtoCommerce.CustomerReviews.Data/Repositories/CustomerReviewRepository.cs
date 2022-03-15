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

        public Task<ReviewRatingCalculateDto[]> GetCustomerReviewsByStoreProductAsync(string storeId, IEnumerable<string> productIds, IEnumerable<int> reviewStatuses)
        {
            var q = CustomerReviews
                   .Where(r => r.StoreId == storeId && reviewStatuses.Contains(r.ReviewStatus));

            if (productIds != null && productIds.Any())
            {
                q = q.Where(r => productIds.Contains(r.ProductId));
            }

            return q.Select(r => new ReviewRatingCalculateDto()
            {
                StoreId = r.StoreId,
                ProductId = r.ProductId,
                Rating = r.Rating,
                CreatedDate = r.CreatedDate
            })
            .ToArrayAsync();

        }
        #endregion

        #region Rating

        public IQueryable<RatingEntity> Ratings => DbContext.Set<RatingEntity>();

        public Task<RatingEntity[]> GetAsync(string storeId, IEnumerable<string> productIds)
        {
            return Ratings
                .Where(x => x.StoreId == storeId && productIds.Contains(x.ProductId))
                .ToArrayAsync();
        }

        public Task<RatingEntity> GetAsync(string storeId, string productId)
        {
            return Ratings.FirstOrDefaultAsync(x => x.StoreId == storeId && x.ProductId == productId);
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

        public Task<RequestReviewEntity> GetRequestReviewAsync(string ProductId, string UserId)
        {
            return RequestReview.FirstOrDefaultAsync(x => x.ProductId == ProductId && x.UserId == UserId);
        }

        public void Delete(RequestReviewEntity entity)
        {
            Remove(entity);
        }

        #endregion
    }
}

