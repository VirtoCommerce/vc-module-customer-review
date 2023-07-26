using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.Models;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.Platform.Caching;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.CustomerReviews.Data.Services
{
    public class CustomerReviewSearchService : SearchService<CustomerReviewSearchCriteria, CustomerReviewSearchResult, CustomerReview, CustomerReviewEntity>, ICustomerReviewSearchService
    {
        private readonly Func<ICustomerReviewRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public CustomerReviewSearchService(
            Func<ICustomerReviewRepository> repositoryFactory,
            IPlatformMemoryCache platformMemoryCache,
            ICustomerReviewService crudService,
            IOptions<CrudOptions> crudOptions)
            : base(repositoryFactory, platformMemoryCache, crudService, crudOptions)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
        }

        public Task<string[]> GetProductIdsOfModifiedReviewsAsync(ChangedReviewsQuery criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetProductIdsOfModifiedReviewsAsync), criteria.ModifiedDate.ToString("s"));
            return _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(GenericCachingRegion<CustomerReview>.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    var ids = await repository.CustomerReviews
                        .Where(r => r.ModifiedDate >= criteria.ModifiedDate)
                        .GroupBy(r => r.EntityId)
                        .Select(x => x.Key)
                        .ToArrayAsync();
                    return ids;
                }
            });
        }

        protected override IQueryable<CustomerReviewEntity> BuildQuery(IRepository repository, CustomerReviewSearchCriteria criteria)
        {
            var query = ((ICustomerReviewRepository)repository).CustomerReviews;

            if (!criteria.EntityIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.EntityIds.Contains(x.EntityId));
            }

            if (!string.IsNullOrEmpty(criteria.EntityType))
            {
                query = query.Where(x => x.EntityType == criteria.EntityType);
            }

            if (!criteria.ReviewStatus.IsNullOrEmpty())
            {
                query = query.Where(r => criteria.ReviewStatus.Contains((CustomerReviewStatus)r.ReviewStatus));
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

            if (criteria.StartDate != null)
            {
                query = query.Where(x => x.ModifiedDate >= criteria.StartDate);
            }

            if (criteria.EndDate != null)
            {
                query = query.Where(x => x.ModifiedDate <= criteria.EndDate);
            }

            if (criteria.StartRating != null)
            {
                query = query.Where(x => x.Rating >= criteria.StartRating);
            }

            if (criteria.EndRating != null)
            {
                query = query.Where(x => x.Rating <= criteria.EndRating);
            }

            return query;
        }

        protected override IList<SortInfo> BuildSortExpression(CustomerReviewSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = nameof(CustomerReview.ModifiedDate),
                        SortDirection = SortDirection.Descending
                    }
                };
            }
            return sortInfos;
        }
    }
}
