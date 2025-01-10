using System.Reflection;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CustomerReviews.Data.Models;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CustomerReviews.Data.Repositories
{
    public class CustomerReviewsDbContext : DbContextBase
    {
#pragma warning disable S109
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

            modelBuilder.Entity<CustomerReviewEntity>().ToTable("CustomerReview").HasKey(x => x.Id);
            modelBuilder.Entity<CustomerReviewEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<CustomerReviewEntity>()
                .HasIndex(x => new { x.StoreId, x.EntityId, x.EntityType, x.ReviewStatus })
                .IsUnique(false);
            modelBuilder.Entity<CustomerReviewEntity>()
                .HasIndex(x => new { x.StoreId, x.EntityId, x.EntityType, x.UserId })
                .IsUnique();

            #endregion

            #region Rating

            modelBuilder.Entity<RatingEntity>().ToTable("Ratings").HasKey(x => x.Id);
            modelBuilder.Entity<RatingEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<RatingEntity>()
                .HasIndex(x => new { x.StoreId, x.EntityId, x.EntityType })
                .IsUnique();
            modelBuilder.Entity<RatingEntity>().Property(x => x.Value).HasPrecision(18, 2);
            #endregion

            #region RequestReview

            modelBuilder.Entity<RequestReviewEntity>().ToTable("RequestReview").HasKey(x => x.Id);
            modelBuilder.Entity<RequestReviewEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<RequestReviewEntity>().
                HasIndex(x => new { x.StoreId, x.EntityId, x.EntityType, x.UserId });

            #endregion

            #region CustomerReviewImage

            modelBuilder.Entity<CustomerReviewImageEntity>().ToTable("CustomerReviewImage").HasKey(x => x.Id);
            modelBuilder.Entity<CustomerReviewImageEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<CustomerReviewImageEntity>().HasOne(m => m.CustomerReview).WithMany(x => x.Images)
                .HasForeignKey(x => x.CustomerReviewId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            #endregion CustomerReviewImage

            base.OnModelCreating(modelBuilder);


            // Allows configuration for an entity type for different database types.
            // Applies configuration from all <see cref="IEntityTypeConfiguration{TEntity}" in VirtoCommerce.CustomerReviews.Data.XXX project. /> 
            switch (this.Database.ProviderName)
            {
                case "Pomelo.EntityFrameworkCore.MySql":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.CustomerReviews.Data.MySql"));
                    break;
                case "Npgsql.EntityFrameworkCore.PostgreSQL":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.CustomerReviews.Data.PostgreSql"));
                    break;
                case "Microsoft.EntityFrameworkCore.SqlServer":
                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.CustomerReviews.Data.SqlServer"));
                    break;
            }
        }
#pragma warning restore S109
    }
}
