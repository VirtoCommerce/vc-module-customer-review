using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;


namespace VirtoCommerce.CustomerReviews.Data.Services
{
    public class CustomerReviewSearchService : ServiceBase, ICustomerReviewSearchService
    {
        private readonly Func<ICustomerReviewRepository> _repositoryFactory;
        private readonly ICustomerReviewService _customerReviewService;

        public CustomerReviewSearchService(Func<ICustomerReviewRepository> repositoryFactory, ICustomerReviewService customerReviewService)
        {
            _repositoryFactory = repositoryFactory;
            _customerReviewService = customerReviewService;
        }

        public async Task<string[]> GetProductIdsOfModifiedReviews(ChangedReviewsQuery query)
        {
            using (var repository = _repositoryFactory())
            {
                var ids = await repository.CustomerReviews
                    .Where(r => r.ModifiedDate >= query.ModifiedDate)
                    .GroupBy(r => r.ProductId)
                    .Select(x => x.Key)
                    .ToArrayAsync();
                return ids;
            }
        }

        public async Task<GenericSearchResult<CustomerReview>> SearchCustomerReviewsAsync(CustomerReviewSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<CustomerReview>();

            using (var repository = _repositoryFactory())
            {
                var query = repository.CustomerReviews;

                if (!criteria.ProductIds.IsNullOrEmpty())
                {
                    query = query.Where(x => criteria.ProductIds.Contains(x.ProductId));
                }

                if (criteria.ReviewStatus.HasValue)
                {
                    var convertedStatus = (byte)criteria.ReviewStatus.Value;
                    query = query.Where(x => x.ReviewStatus == convertedStatus);
                }

                if (!criteria.SearchPhrase.IsNullOrEmpty())
                {
                    query = query.Where(x => x.Review.Contains(criteria.SearchPhrase));
                }

                if (!criteria.StoreId.IsNullOrEmpty())
                {
                    query = query.Where(x => x.StoreId == criteria.StoreId);
                }

                if (criteria.ModifiedDate.HasValue)
                {
                    query = query.Where(x => x.ModifiedDate >= criteria.ModifiedDate.Value);
                }

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = "CreatedDate", SortDirection = SortDirection.Descending } };
                }
                query = query.OrderBySortInfos(sortInfos);

                retVal.TotalCount = query.Count();

                var customerReviewIds = await query.Skip(criteria.Skip)
                                 .Take(criteria.Take)
                                 .Select(x => x.Id)
                                 .ToListAsync();

                var priorResults = await _customerReviewService.GetByIdsAsync(customerReviewIds);
                retVal.Results = priorResults.OrderBy(x => customerReviewIds.IndexOf(x.Id)).ToList();
                return retVal;
            }
        }

    }
}
