using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CustomerReviews.Data.Models;

namespace VirtoCommerce.CustomerReviews.Data.Repositories
{
    public class CustomerReviewsDbContext : DbContextWithTriggers
    {
        public CustomerReviewsDbContext(DbContextOptions<CustomerReviewsDbContext> options)
            : base(options)
        {
        }

        protected CustomerReviewsDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region CustomerReview

            modelBuilder.Entity<CustomerReviewEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<CustomerReviewEntity>().ToTable("CustomerReview");
            modelBuilder.Entity<CustomerReviewEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<CustomerReviewEntity>()
                .HasIndex(x => new { x.StoreId, x.EntityId, x.EntityType, x.ReviewStatus })
                .IsUnique(false);
            modelBuilder.Entity<CustomerReviewEntity>()
                .HasIndex(x => new { x.StoreId, x.EntityId, x.EntityType, x.UserId })
                .IsUnique();

            #endregion

            #region Rating

            modelBuilder.Entity<RatingEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<RatingEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<RatingEntity>().ToTable("Ratings");
            modelBuilder.Entity<RatingEntity>()
                .HasIndex(x => new { x.StoreId, x.EntityId, x.EntityType })
                .IsUnique();

            #endregion

            #region RequestReview

            modelBuilder.Entity<RequestReviewEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<RequestReviewEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<RequestReviewEntity>().ToTable("RequestReview");
            modelBuilder.Entity<RequestReviewEntity>().
                HasIndex(x => new { x.StoreId, x.EntityId, x.EntityType, x.UserId });

            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
