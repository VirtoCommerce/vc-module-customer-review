using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Moq;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.Services;
using VirtoCommerce.Platform.Core.Caching;
using Xunit;

namespace VirtoCommerce.CustomerReviews.Test
{
    [Trait("Category", "IntegrationTest")]
    public class CustomerReviewsTests
    {
        private const string _entityId = "TestProductId";
        private const string _entityType = "Product";
        private const string _entityName = "Test product";
        private const string _customerReviewId = "TestId";

        private readonly Mock<IPlatformMemoryCache> _platformMemoryCacheMock;
        private readonly Mock<ICacheEntry> _cacheEntryMock;
        private ICustomerReviewService _customerReviewService;
        private ICustomerReviewSearchService _customerReviewSearchService;

        public CustomerReviewsTests()
        {
            _cacheEntryMock = new Mock<ICacheEntry>();
            _cacheEntryMock.SetupGet(c => c.ExpirationTokens).Returns(new List<IChangeToken>());
            _platformMemoryCacheMock = new Mock<IPlatformMemoryCache>();
            _platformMemoryCacheMock.Setup(x => x.GetDefaultCacheEntryOptions()).Returns(() => new MemoryCacheEntryOptions());
            var cacheKeyCrud = CacheKey.With(typeof(CustomerReviewService), "GetAsync", string.Join("-", _customerReviewId), null);
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKeyCrud)).Returns(_cacheEntryMock.Object);
        }

        [Fact]
        public async Task CanDoCrudAndSearch()
        {
            MockServices(_customerReviewId, _entityId, _entityType);

            // Create
            var item = new CustomerReview
            {
                Id = _customerReviewId,
                EntityId = _entityId,
                EntityType = _entityType,
                EntityName = _entityName,
                CreatedDate = DateTime.Now,
                CreatedBy = "initial data seed",
                UserName = "John Doe",
                UserId = "TestUserId",
                Review = "Liked that",
                Rating = 5,
                StoreId = "Electronics",
                Title = "my first review",
                ReviewStatus = CustomerReviewStatus.New
            };

            await _customerReviewService.SaveChangesAsync(new[] { item });

            var result = await _customerReviewService.GetAsync(new[] { _customerReviewId });
            Assert.Single(result);
            item = result.First();
            Assert.Equal(_customerReviewId, item.Id);

            // Update
            var updatedContent = "Updated content";
            Assert.NotEqual(updatedContent, item.Review);

            item.Review = updatedContent;
            await _customerReviewService.SaveChangesAsync(new[] { item });

            result = await _customerReviewService.GetAsync(new[] { _customerReviewId });
            Assert.Single(result);

            item = result.First();
            Assert.Equal(updatedContent, item.Review);

            var criteria = new CustomerReviewSearchCriteria { EntityIds = new[] { _entityId }, EntityType = _entityType };
            var cacheKeySearch = CacheKey.With(typeof(CustomerReviewSearchService), "SearchAsync", criteria.GetCacheKey());
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKeySearch)).Returns(_cacheEntryMock.Object);

            var searchResult = await _customerReviewSearchService.SearchAsync(criteria);
            Assert.NotNull(searchResult);
            Assert.Equal(1, searchResult.TotalCount);
            Assert.Single(searchResult.Results);

            // Delete
            await _customerReviewService.DeleteAsync(new[] { _customerReviewId });

            var getByIdsResult = await _customerReviewService.GetAsync(new[] { _customerReviewId });
            Assert.NotNull(getByIdsResult);
        }

        private void MockServices(string customerReviewId, string entityId, string entityType)
        {
            var customerReviewService = new Mock<ICustomerReviewService>();

            var customerReviews = TestHelper.LoadFromJsonFile<CustomerReview[]>(@"customerReviews.json");

            customerReviewService
                .Setup(x => x.GetAsync(new[] { customerReviewId }, It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.FromResult((IList<CustomerReview>)customerReviews.Where(x => x.Id == customerReviewId).ToList()));

            customerReviewService
                .Setup(x => x.SaveChangesAsync(It.IsAny<CustomerReview[]>()))
                .Returns(Task.CompletedTask);

            customerReviewService
                .Setup(x => x.DeleteAsync(It.IsAny<List<string>>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            _customerReviewService = customerReviewService.Object;

            var customerReviewSearchService = new Mock<ICustomerReviewSearchService>();

            customerReviewSearchService
                .Setup(x => x.SearchAsync(new CustomerReviewSearchCriteria { EntityIds = new[] { entityId }, EntityType = entityType }, It.IsAny<bool>()))
                .Returns(Task.FromResult(new CustomerReviewSearchResult
                {
                    Results = customerReviews.Where(x => x.EntityId == entityId && x.EntityType == entityType).ToList(),
                    TotalCount = customerReviews.Count(x => x.EntityId == entityId && x.EntityType == entityType)
                }));

            _customerReviewSearchService = customerReviewSearchService.Object;
        }
    }
}
