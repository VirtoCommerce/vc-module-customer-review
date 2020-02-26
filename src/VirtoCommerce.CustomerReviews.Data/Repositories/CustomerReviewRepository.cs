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
        
        public Task<CustomerReviewEntity[]> GetByIdsAsync(IEnumerable<string> ids)
        {
            return CustomerReviews.Where(x => ids.Contains(x.Id)).ToArrayAsync();
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

        public async Task<RatingEntity[]> GetAsync(string storeId, IEnumerable<string> productIds)
        {
            return await Ratings
                .Where(x => x.StoreId == storeId && productIds.Contains(x.ProductId))
                .ToArrayAsync();
        }

        public async Task<RatingEntity> GetAsync(string storeId, string productId)
        {
            return await Ratings.FirstOrDefaultAsync(x => x.StoreId == storeId && x.ProductId == productId);
        }

        public void Delete(RatingEntity entity)
        {
            Remove(entity);
        }

        #endregion
    }
}

