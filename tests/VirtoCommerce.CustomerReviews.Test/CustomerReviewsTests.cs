//using System;
//using System.Data.Entity;
//using System.Linq;
//using System.Threading.Tasks;
//using VirtoCommerce.CustomerReviews.Core.Models;
//using VirtoCommerce.CustomerReviews.Core.Services;
//using VirtoCommerce.CustomerReviews.Data.Migrations;
//using VirtoCommerce.CustomerReviews.Data.Repositories;
//using VirtoCommerce.CustomerReviews.Data.Services;
//using VirtoCommerce.Platform.Data.Infrastructure;
//using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
//using VirtoCommerce.Platform.Testing.Bases;
//using Xunit;

//namespace VirtoCommerce.CustomerReviews.Test
//{
//    public class CustomerReviewsTests : FunctionalTestBase
//    {
//        private const string ProductId = "testProductId";
//        private const string CustomerReviewId = "testId";

//        public CustomerReviewsTests()
//        {
//            ConnectionString = "VirtoCommerce";
//        }

//        [Fact]
//        public async Task CanDoCRUDandSearch()
//        {
//            // Read non-existing item
//            var getByIdsResult = await CustomerReviewService.GetByIdsAsync(new[] { CustomerReviewId });
//            Assert.NotNull(getByIdsResult);
//            Assert.Empty(getByIdsResult);

//            // Create
//            var item = new CustomerReview
//            {
//                Id = CustomerReviewId,
//                ProductId = ProductId,
//                CreatedDate = DateTime.Now,
//                CreatedBy = "initial data seed",
//                UserName = "John Doe",
//                UserId = "dsdsd12dfsd",
//                Review = "Liked that",
//                Rating = 5,
//                StoreId = "Electronics",
//                Title = "my first review",
//                ReviewStatus = CustomerReviewStatus.New
//            };

//            await CustomerReviewService.SaveCustomerReviewsAsync(new[] { item });

//            getByIdsResult = await CustomerReviewService.GetByIdsAsync(new[] { CustomerReviewId });
//            Assert.Single(getByIdsResult);

//            item = getByIdsResult.First();
//            Assert.Equal(CustomerReviewId, item.Id);

//            // Update
//            var updatedContent = "Updated content";
//            Assert.NotEqual(updatedContent, item.Review);

//            item.Review = updatedContent;
//            await CustomerReviewService.SaveCustomerReviewsAsync(new[] { item });
//            getByIdsResult = await CustomerReviewService.GetByIdsAsync(new[] { CustomerReviewId });
//            Assert.Single(getByIdsResult);

//            item = getByIdsResult.First();
//            Assert.Equal(updatedContent, item.Review);

//            var criteria = new CustomerReviewSearchCriteria { ProductIds = new[] { ProductId } };
//            var searchResult = await CustomerReviewSearchService.SearchCustomerReviewsAsync(criteria);

//            Assert.NotNull(searchResult);
//            Assert.Equal(1, searchResult.TotalCount);
//            Assert.Single(searchResult.Results);

//            // Delete
//            await CanDeleteCustomerReviews();
//        }

//        [Fact]
//        public async Task CanDeleteCustomerReviews()
//        {
//            await CustomerReviewService.DeleteCustomerReviewsAsync(new[] { CustomerReviewId });

//            var getByIdsResult = await CustomerReviewService.GetByIdsAsync(new[] { CustomerReviewId });
//            Assert.NotNull(getByIdsResult);
//            Assert.Empty(getByIdsResult);
//        }

//        private ICustomerReviewSearchService CustomerReviewSearchService
//        {
//            get
//            {
//                return new CustomerReviewSearchService(GetRepository, CustomerReviewService);
//            }
//        }
//        private ICustomerReviewService CustomerReviewService
//        {
//            get
//            {
//                return new CustomerReviewService(GetRepository, null);
//            }
//        }


//        protected ICustomerReviewRepository GetRepository()
//        {
//            var repository = new CustomerReviewRepository(ConnectionString, new EntityPrimaryKeyGeneratorInterceptor(), new AuditableInterceptor(null));
//            EnsureDatabaseInitialized(() => new CustomerReviewRepository(ConnectionString), () => Database.SetInitializer(new SetupDatabaseInitializer<CustomerReviewRepository, Configuration>()));
//            return repository;
//        }
//    }
//}
