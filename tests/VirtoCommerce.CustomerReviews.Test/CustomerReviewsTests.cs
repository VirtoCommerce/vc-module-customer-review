using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Moq;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.CustomerReviews.Data.Services;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Events;
using Xunit;

namespace VirtoCommerce.CustomerReviews.Test
{
    [Trait("Category", "IntegrationTest")]
    public class CustomerReviewsTests
    {
        private const string ProductId = "testProductId";
        private const string CustomerReviewId = "testId";

        private readonly Mock<IPlatformMemoryCache> _platformMemoryCacheMock;
        private readonly Mock<ICacheEntry> _cacheEntryMock;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly DbContextOptions<CustomerReviewsDbContext> _dbContextOptions;

        public CustomerReviewsTests()
        {
            _cacheEntryMock = new Mock<ICacheEntry>();
            _cacheEntryMock.SetupGet(c => c.ExpirationTokens).Returns(new List<IChangeToken>());
            _platformMemoryCacheMock = new Mock<IPlatformMemoryCache>();
            _eventPublisherMock = new Mock<IEventPublisher>();
            _dbContextOptions = new DbContextOptionsBuilder<CustomerReviewsDbContext>()
                .UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30")
                .Options;

            var cacheKey = CacheKey.With(typeof(CustomerReviewService), "GetByIdsAsync", string.Join("-", CustomerReviewId));
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKey)).Returns(_cacheEntryMock.Object);
        }

        [Fact]
        public async Task CanDoCRUDandSearch()
        {
            CustomerReview[] result;

            // Read non-existing item
            await using (var context = new CustomerReviewsDbContext(_dbContextOptions))
            {
                result = await CustomerReviewService(context).GetByIdsAsync(new[] { CustomerReviewId });
                Assert.NotNull(result);
                Assert.Empty(result);
            }



            // Create
            var item = new CustomerReview
            {
                Id = CustomerReviewId,
                ProductId = ProductId,
                CreatedDate = DateTime.Now,
                CreatedBy = "initial data seed",
                UserName = "John Doe",
                UserId = "dsdsd12dfsd",
                Review = "Liked that",
                Rating = 5,
                StoreId = "Electronics",
                Title = "my first review",
                ReviewStatus = CustomerReviewStatus.New
            };

            await using (var context = new CustomerReviewsDbContext(_dbContextOptions))
            {
                await CustomerReviewService(context).SaveCustomerReviewsAsync(new[] {item});
            }

            await using (var context = new CustomerReviewsDbContext(_dbContextOptions))
            {
                result = await CustomerReviewService(context).GetByIdsAsync(new[] { CustomerReviewId });
            }

            Assert.Single(result);
            item = result.First();
            Assert.Equal(CustomerReviewId, item.Id);

            // Update
            var updatedContent = "Updated content";
            Assert.NotEqual(updatedContent, item.Review);

            item.Review = updatedContent;
            await using (var context = new CustomerReviewsDbContext(_dbContextOptions))
            {
                await CustomerReviewService(context).SaveCustomerReviewsAsync(new[] { item });
            }

            await using (var context = new CustomerReviewsDbContext(_dbContextOptions))
            {
                result = await CustomerReviewService(context).GetByIdsAsync(new[] { CustomerReviewId });
                Assert.Single(result);
            }

            item = result.First();
            Assert.Equal(updatedContent, item.Review);

            var criteria = new CustomerReviewSearchCriteria { ProductIds = new[] { ProductId } };
            await using (var context = new CustomerReviewsDbContext(_dbContextOptions))
            {
                var searchResult = await CustomerReviewSearchService(context).SearchCustomerReviewsAsync(criteria);
                Assert.NotNull(searchResult);
                Assert.Equal(1, searchResult.TotalCount);
                Assert.Single(searchResult.Results);
            }

            // Delete
            await CanDeleteCustomerReviews();
        }

        [Fact]
        public async Task CanDeleteCustomerReviews()
        {
            using (var context = new CustomerReviewsDbContext(_dbContextOptions))
            {
                await CustomerReviewService(context).DeleteCustomerReviewsAsync(new[] { CustomerReviewId });

                var getByIdsResult = await CustomerReviewService(context).GetByIdsAsync(new[] { CustomerReviewId });
                Assert.NotNull(getByIdsResult);
                Assert.Empty(getByIdsResult);
            }
                
        }

        private ICustomerReviewSearchService CustomerReviewSearchService(CustomerReviewsDbContext customerReviewsDbContext)
        {
            return new CustomerReviewSearchService(() => GetRepository(customerReviewsDbContext), CustomerReviewService(customerReviewsDbContext), _platformMemoryCacheMock.Object);
        }

        private ICustomerReviewService CustomerReviewService(CustomerReviewsDbContext customerReviewsDbContext)
        {
            return new CustomerReviewService(() => GetRepository(customerReviewsDbContext), _eventPublisherMock.Object, _platformMemoryCacheMock.Object);
        }


        protected ICustomerReviewRepository GetRepository(CustomerReviewsDbContext customerReviewsDbContext)
        {
            var repository = new CustomerReviewRepository(customerReviewsDbContext);
            return repository;
        }
    }
}
