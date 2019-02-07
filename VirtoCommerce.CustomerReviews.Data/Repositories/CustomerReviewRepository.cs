using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Data.Models;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.CustomerReviews.Data.Repositories
{
    public class CustomerReviewRepository : EFRepositoryBase, ICustomerReviewRepository
    {
        public CustomerReviewRepository()
        {
        }

        public CustomerReviewRepository(string nameOrConnectionString, params IInterceptor[] interceptors)
            : base(nameOrConnectionString, null, interceptors)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        #region CustomerReviews
        public IQueryable<CustomerReviewEntity> CustomerReviews => GetAsQueryable<CustomerReviewEntity>();


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

        public Task<ReviewRatingCalculateDto[]> GetCustomerReviewsByStoreProductAsync(string storeId, IEnumerable<string> productIds, IEnumerable<byte> reviewStatuses)
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

        public IQueryable<RatingEntity> Ratings => GetAsQueryable<RatingEntity>();

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


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region CustomerReview

            modelBuilder.Entity<CustomerReviewEntity>()
                .ToTable("CustomerReview")
                .HasKey(x => x.Id)
                .Property(x => x.Id);
            modelBuilder.Entity<CustomerReviewEntity>()
                .HasIndex(x => new { x.StoreId, x.ProductId, x.ReviewStatus })
                .IsUnique(false);

            #endregion

            #region Rating

            modelBuilder.Entity<RatingEntity>()
                .ToTable("Ratings")
                .HasKey(x => x.Id)
                .Property(x => x.Id);

            modelBuilder.Entity<RatingEntity>()
                .HasIndex(x => new { x.StoreId, x.ProductId })
                .IsUnique();

            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}

