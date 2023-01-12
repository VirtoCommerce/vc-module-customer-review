using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Moq;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Data.Models;
using VirtoCommerce.CustomerReviews.Data.Services;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.GenericCrud;
using Xunit;

namespace VirtoCommerce.CustomerReviews.Test
{
    [Trait("Category", "IntegrationTest")]
    public class CustomerReviewsTests
    {
        private const string EntityId = "TestProductId";
        private const string EntityType = "Product";
        private const string EntityName = "Test product";
        private const string CustomerReviewId = "TestId";

        private readonly Mock<IPlatformMemoryCache> _platformMemoryCacheMock;
        private readonly Mock<ICacheEntry> _cacheEntryMock;
        private ICrudService<CustomerReview> _customerReviewService;
        private ISearchService<CustomerReviewSearchCriteria, CustomerReviewSearchResult, CustomerReview> _customerReviewSearchService;

        public CustomerReviewsTests()
        {
            _cacheEntryMock = new Mock<ICacheEntry>();
            _cacheEntryMock.SetupGet(c => c.ExpirationTokens).Returns(new List<IChangeToken>());
            _platformMemoryCacheMock = new Mock<IPlatformMemoryCache>();
            _platformMemoryCacheMock.Setup(x => x.GetDefaultCacheEntryOptions()).Returns(() => new MemoryCacheEntryOptions());
            var cacheKeyCRUD = CacheKey.With(typeof(CustomerReviewService), "GetAsync", string.Join("-", CustomerReviewId), null);
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKeyCRUD)).Returns(_cacheEntryMock.Object);
        }

        [Fact]
        public async Task CanDoCRUDandSearch()
        {
            MockServices(CustomerReviewId, EntityId, EntityType);

            IEnumerable<CustomerReview> result;

            // Create
            var item = new CustomerReview
            {
                Id = CustomerReviewId,
                EntityId = EntityId,
                EntityType = EntityType,
                EntityName = EntityName,
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

            await _customerReviewService.SaveChangesAsync(new[] { item });

            result = await _customerReviewService.GetAsync(new List<string>() { CustomerReviewId });
            Assert.Single(result);
            item = result.First();
            Assert.Equal(CustomerReviewId, item.Id);

            // Update
            var updatedContent = "Updated content";
            Assert.NotEqual(updatedContent, item.Review);

            item.Review = updatedContent;
            await _customerReviewService.SaveChangesAsync(new[] { item });

            result = await _customerReviewService.GetAsync(new List<string>() { CustomerReviewId });
            Assert.Single(result);

            item = result.First();
            Assert.Equal(updatedContent, item.Review);

            var criteria = new CustomerReviewSearchCriteria { EntityIds = new[] { EntityId }, EntityType = EntityType };
            var cacheKeySearch = CacheKey.With(typeof(CustomerReviewSearchService), "SearchAsync", criteria.GetCacheKey());
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKeySearch)).Returns(_cacheEntryMock.Object);

            var searchResult = await _customerReviewSearchService.SearchAsync(criteria);
            Assert.NotNull(searchResult);
            Assert.Equal(1, searchResult.TotalCount);
            Assert.Single(searchResult.Results);

            // Delete
            await _customerReviewService.DeleteAsync(new[] { CustomerReviewId });

            var getByIdsResult = await _customerReviewService.GetAsync(new List<string>() { CustomerReviewId });
            Assert.NotNull(getByIdsResult);
        }

        private void MockServices(string customerReviewId, string entityId, string entityType)
        {
            var customerReviewService = new Mock<ICrudService<CustomerReview>>();

            CustomerReview[] customerReviews = TestHelper.LoadFromJsonFile<CustomerReview[]>(@"customerReviews.json");
            RatingEntity[] ratings = TestHelper.LoadFromJsonFile<RatingEntity[]>(@"ratings.json");

            customerReviewService
                .Setup(x => x.GetAsync(new List<string> { customerReviewId }, It.IsAny<string>()))
                .Returns(Task.FromResult((IReadOnlyCollection<CustomerReview>)new ReadOnlyCollection<CustomerReview>(customerReviews.Where(x => x.Id == customerReviewId).ToList())));

            customerReviewService
                .Setup(x => x.SaveChangesAsync(It.IsAny<CustomerReview[]>()))
                .Returns(Task.CompletedTask);

            customerReviewService
                .Setup(x => x.DeleteAsync(It.IsAny<List<string>>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            _customerReviewService = customerReviewService.Object;

            var customerReviewSearchService = new Mock<ISearchService<CustomerReviewSearchCriteria, CustomerReviewSearchResult, CustomerReview>>();

            customerReviewSearchService
                .Setup(x => x.SearchAsync(new CustomerReviewSearchCriteria { EntityIds = new[] { entityId }, EntityType = entityType }))
                .Returns(Task.FromResult(new CustomerReviewSearchResult
                {
                    Results = customerReviews.Where(x => x.EntityId == entityId && x.EntityType == entityType).ToList(),
                    TotalCount = customerReviews.Where(x => x.EntityId == entityId && x.EntityType == entityType).Count()
                }));

            _customerReviewSearchService = customerReviewSearchService.Object;
        }
    }
}
