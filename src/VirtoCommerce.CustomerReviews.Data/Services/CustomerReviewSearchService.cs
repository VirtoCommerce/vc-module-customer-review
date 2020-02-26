using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.Caching;
using VirtoCommerce.CustomerReviews.Data.Models;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;


namespace VirtoCommerce.CustomerReviews.Data.Services
{
    public class CustomerReviewSearchService : ICustomerReviewSearchService
    {
        private readonly Func<ICustomerReviewRepository> _repositoryFactory;
        private readonly ICustomerReviewService _customerReviewService;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public CustomerReviewSearchService(Func<ICustomerReviewRepository> repositoryFactory, ICustomerReviewService customerReviewService, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _customerReviewService = customerReviewService;
            _platformMemoryCache = platformMemoryCache;
        }

        public async Task<string[]> GetProductIdsOfModifiedReviewsAsync(ChangedReviewsQuery criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetProductIdsOfModifiedReviewsAsync), criteria.ModifiedDate.ToString("s"));
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CustomerReviewCacheRegion.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    var ids = await repository.CustomerReviews
                        .Where(r => r.ModifiedDate >= criteria.ModifiedDate)
                        .GroupBy(r => r.ProductId)
                        .Select(x => x.Key)
                        .ToArrayAsync();
                    return ids;
                }
            });
        }

        public async Task<CustomerReviewSearchResult> SearchCustomerReviewsAsync(CustomerReviewSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchCustomerReviewsAsync), criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CustomerReviewCacheRegion.CreateChangeToken());
                var result = AbstractTypeFactory<CustomerReviewSearchResult>.TryCreateInstance();

                using (var repository = _repositoryFactory())
                {
                    var query = BuildQuery(repository, criteria);
                    var sortInfos = BuildSortExpression(criteria);

                    result.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        var customerReviewIds = await query.OrderBySortInfos(sortInfos).Skip(criteria.Skip)
                            .Take(criteria.Take)
                            .Select(x => x.Id)
                            .ToListAsync();

                        var priorResults = await _customerReviewService.GetByIdsAsync(customerReviewIds);
                        result.Results = priorResults.OrderBy(x => customerReviewIds.IndexOf(x.Id)).ToList();
                    }
                        
                    return result;
                }
            });
        }


        protected virtual IQueryable<CustomerReviewEntity> BuildQuery(ICustomerReviewRepository repository,
            CustomerReviewSearchCriteria criteria)
        {
            var query = repository.CustomerReviews;

            if (!criteria.ProductIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.ProductIds.Contains(x.ProductId));
            }

            if (!criteria.ReviewStatus.IsNullOrEmpty())
            {
                query = query.Where(r => criteria.ReviewStatus.Contains(r.ReviewStatus));
            }

            if (!criteria.SearchPhrase.IsNullOrEmpty())
            {
                query = query.Where(x => x.Review.Contains(criteria.SearchPhrase));
            }

            if (!criteria.StoreId.IsNullOrEmpty())
            {
                query = query.Where(x => x.StoreId == criteria.StoreId);
            }

            if (!criteria.UserId.IsNullOrEmpty())
            {
                query = query.Where(x => x.UserId == criteria.UserId);
            }

            if (criteria.ModifiedDate.HasValue)
            {
                query = query.Where(x => x.ModifiedDate >= criteria.ModifiedDate.Value);
            }

            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(CustomerReviewSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] {
                    new SortInfo { SortColumn = nameof(CustomerReview.CreatedDate), SortDirection = SortDirection.Descending },
                };
            }
            return sortInfos;
        }

    }
}
