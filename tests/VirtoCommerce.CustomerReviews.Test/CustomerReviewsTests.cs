using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Moq;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.CustomerReviews.Data.Services;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.GenericCrud;
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
                .UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3;Persist Security Info=True;User ID=virto;Password=virto;Connect Timeout=30")
                .Options;

            _platformMemoryCacheMock.Setup(x => x.GetDefaultCacheEntryOptions()).Returns(() => new MemoryCacheEntryOptions());


            var cacheKeyCRUD = CacheKey.With(typeof(CustomerReviewService), "GetAsync", string.Join("-", CustomerReviewId), null);
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKeyCRUD)).Returns(_cacheEntryMock.Object);
        }

        [Fact]
        public async Task CanDoCRUDandSearch()
        {
            IEnumerable<CustomerReview> result;

            // Read non-existing item

            result = await CustomerReviewService().GetAsync(new List<string>() { CustomerReviewId });
            Assert.NotNull(result);
            Assert.Empty(result);



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

            await CustomerReviewService().SaveChangesAsync(new[] { item });

            result = await CustomerReviewService().GetAsync(new List<string>() { CustomerReviewId });
            Assert.Single(result);
            item = result.First();
            Assert.Equal(CustomerReviewId, item.Id);

            // Update
            var updatedContent = "Updated content";
            Assert.NotEqual(updatedContent, item.Review);

            item.Review = updatedContent;
            await CustomerReviewService().SaveChangesAsync(new[] { item });

            result = await CustomerReviewService().GetAsync(new List<string>() { CustomerReviewId });
            Assert.Single(result);

            item = result.First();
            Assert.Equal(updatedContent, item.Review);

            var criteria = new CustomerReviewSearchCriteria { ProductIds = new[] { ProductId } };
            var cacheKeySearch = CacheKey.With(typeof(CustomerReviewSearchService), "SearchAsync", criteria.GetCacheKey());
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKeySearch)).Returns(_cacheEntryMock.Object);

            var searchResult = await CustomerReviewSearchService().SearchAsync(criteria);
            Assert.NotNull(searchResult);
            Assert.Equal(1, searchResult.TotalCount);
            Assert.Single(searchResult.Results);

            // Delete
            await CustomerReviewService().DeleteAsync(new[] { CustomerReviewId });

            var getByIdsResult = await CustomerReviewService().GetAsync(new List<string>() { CustomerReviewId });
            Assert.NotNull(getByIdsResult);
            Assert.Empty(getByIdsResult);
        }


        private ISearchService<CustomerReviewSearchCriteria, CustomerReviewSearchResult, CustomerReview> CustomerReviewSearchService()
        {
            return new CustomerReviewSearchService(() => GetRepository(new CustomerReviewsDbContext(_dbContextOptions)), _platformMemoryCacheMock.Object, (ICustomerReviewService)CustomerReviewService());
        }

        private ICrudService<CustomerReview> CustomerReviewService()
        {
            return new CustomerReviewService(() => GetRepository(new CustomerReviewsDbContext(_dbContextOptions)), _platformMemoryCacheMock.Object, _eventPublisherMock.Object);
        }


        protected ICustomerReviewRepository GetRepository(CustomerReviewsDbContext customerReviewsDbContext)
        {
            var repository = new CustomerReviewRepository(customerReviewsDbContext);
            return repository;
        }
    }
}
