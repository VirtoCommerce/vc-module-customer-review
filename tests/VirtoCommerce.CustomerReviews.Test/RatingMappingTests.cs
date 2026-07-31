using System.Threading.Tasks;
using Moq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;
using VirtoCommerce.CustomerReviews.ExperienceApi.Mapping;
using VirtoCommerce.CustomerReviews.ExperienceApi.Middleware;
using VirtoCommerce.ProfileExperienceApiModule.Data;
using VirtoCommerce.ProfileExperienceApiModule.Data.Aggregates.Vendor;
using VirtoCommerce.Xapi.Core.Models;
using VirtoCommerce.XCatalog.Core.Models;
using VirtoCommerce.XCatalog.Core.Queries;
using Xunit;

namespace VirtoCommerce.CustomerReviews.Test
{
    [Trait("Category", "UnitTest")]
    public class RatingMappingTests
    {
        [Fact]
        public void Maps_RatingEntityDto_To_ExpRating()
        {
            var source = new RatingEntityDto
            {
                EntityId = "P1",
                EntityType = "Product",
                Value = 4.5m,
                ReviewCount = 10,
            };

            var result = source.ToExpRating();

            Assert.NotNull(result);
            Assert.Equal(source.Value, result.Value);
            Assert.Equal(source.ReviewCount, result.ReviewCount);
        }

        [Fact]
        public void Maps_RatingEntityStoreDto_To_ExpVendorRating()
        {
            var source = new RatingEntityStoreDto
            {
                StoreId = "Store1",
                StoreName = "Store One",
                EntityId = "V1",
                EntityType = "Vendor",
                Value = 3.2m,
                ReviewCount = 7,
            };

            var result = source.ToExpVendorRating();

            Assert.NotNull(result);
            Assert.Equal(source.StoreId, result.StoreId);
            Assert.Equal(source.Value, result.Value);
            Assert.Equal(source.ReviewCount, result.ReviewCount);
        }

        [Fact]
        public void Maps_CreateReviewCommand_To_CustomerReview()
        {
            var source = new CreateReviewCommand
            {
                StoreId = "Store1",
                EntityId = "P1",
                EntityType = "Product",
                UserId = "U1",
                Review = "Great product",
                Rating = 5,
                ImageUrls = ["/api/files/img1"],
            };

            var result = source.ToCustomerReview();

            Assert.NotNull(result);
            Assert.Equal(source.StoreId, result.StoreId);
            Assert.Equal(source.EntityId, result.EntityId);
            Assert.Equal(source.EntityType, result.EntityType);
            Assert.Equal(source.UserId, result.UserId);
            Assert.Equal(source.Review, result.Review);
            Assert.Equal(source.Rating, result.Rating);
        }

        [Fact]
        public async Task EvalVendorRatingMiddleware_Maps_Ratings_Onto_Vendor()
        {
            var vendorAggregate = new VendorAggregate
            {
                Member = new Contact { Id = "V1", MemberType = "Vendor" },
            };

            var ratings = new[]
            {
                new RatingEntityStoreDto { StoreId = "Store1", EntityId = "V1", EntityType = "Vendor", Value = 4.0m, ReviewCount = 3 },
                new RatingEntityStoreDto { StoreId = "Store2", EntityId = "V1", EntityType = "Vendor", Value = 5.0m, ReviewCount = 1 },
            };

            var ratingServiceMock = new Mock<IRatingService>();
            ratingServiceMock
                .Setup(x => x.GetRatingsAsync(new[] { "V1" }, "Vendor"))
                .ReturnsAsync(ratings);

            var middleware = new EvalVendorRatingMiddleware(ratingServiceMock.Object);

            await middleware.Run(vendorAggregate, _ => Task.CompletedTask);

            Assert.NotNull(vendorAggregate.Ratings);
            var mapped = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<ExpVendorRating>>(vendorAggregate.Ratings);
            var mappedArray = System.Linq.Enumerable.ToArray(mapped);
            Assert.Equal(2, mappedArray.Length);
            Assert.Contains(mappedArray, r => r.StoreId == "Store1" && r.Value == 4.0m && r.ReviewCount == 3);
            Assert.Contains(mappedArray, r => r.StoreId == "Store2" && r.Value == 5.0m && r.ReviewCount == 1);
        }

        [Fact]
        public async Task EvalProductRatingMiddleware_Maps_Rating_Onto_Product()
        {
            var product = new ExpProduct { IndexedProduct = new CatalogProduct { Id = "P1" } };
            var query = new SearchProductQuery { StoreId = "Store1", IncludeFields = ["rating"] };
            var response = new SearchProductResponse
            {
                Query = query,
                Results = [product],
            };

            var ratingServiceMock = new Mock<IRatingService>();
            ratingServiceMock
                .Setup(x => x.GetForStoreAsync("Store1", new[] { "P1" }, ReviewEntityTypes.Product))
                .ReturnsAsync([new RatingEntityDto { EntityId = "P1", EntityType = ReviewEntityTypes.Product, Value = 4.2m, ReviewCount = 8 }]);

            var middleware = new EvalProductRatingMiddleware(ratingServiceMock.Object);

            await middleware.Run(response, _ => Task.CompletedTask);

            Assert.NotNull(product.Rating);
            Assert.Equal(4.2m, product.Rating.Value);
            Assert.Equal(8, product.Rating.ReviewCount);
        }

        [Fact]
        public async Task EvalProductVendorRatingMiddleware_Maps_Rating_Onto_Product_Vendor()
        {
            var vendor = new ExpVendor { Id = "V1", Type = "Vendor" };
            var product = new ExpProduct { IndexedProduct = new CatalogProduct { Id = "P1" }, Vendor = vendor };
            var query = new SearchProductQuery { StoreId = "Store1", IncludeFields = ["rating"] };
            var response = new SearchProductResponse
            {
                Query = query,
                Results = [product],
            };

            var ratingServiceMock = new Mock<IRatingService>();
            ratingServiceMock
                .Setup(x => x.GetForStoreAsync("Store1", new[] { "V1" }, "Vendor"))
                .ReturnsAsync([new RatingEntityDto { EntityId = "V1", EntityType = "Vendor", Value = 3.9m, ReviewCount = 2 }]);

            var middleware = new EvalProductVendorRatingMiddleware(ratingServiceMock.Object);

            await middleware.Run(response, _ => Task.CompletedTask);

            Assert.NotNull(product.Vendor.Rating);
            Assert.Equal(3.9m, product.Vendor.Rating.Value);
            Assert.Equal(2, product.Vendor.Rating.ReviewCount);
        }
    }
}
