using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Queries;

public class CustomerReviewsQueryHandler: IQueryHandler<CustomerReviewsQuery, CustomerReviewSearchResult>
{
    private readonly ISearchService<CustomerReviewSearchCriteria, CustomerReviewSearchResult, CustomerReview> _customerReviewSearchService;
    private readonly ISearchPhraseParser _phraseParser;

    public CustomerReviewsQueryHandler(
        ISearchService<CustomerReviewSearchCriteria, CustomerReviewSearchResult, CustomerReview> customerReviewSearchService,
        ISearchPhraseParser phraseParser)
    {
        _customerReviewSearchService = customerReviewSearchService;
        _phraseParser = phraseParser;
    }

    public virtual async Task<CustomerReviewSearchResult> Handle(CustomerReviewsQuery request, CancellationToken cancellationToken)
    {
        var criteria = GetSearchCriteria(request);
        var result = await _customerReviewSearchService.SearchAsync(criteria);

        return result;
    }

    protected virtual CustomerReviewSearchCriteria GetSearchCriteria(CustomerReviewsQuery request)
    {
        var criteria = request.GetSearchCriteria<CustomerReviewSearchCriteria>();
        criteria.StoreId = request.StoreId;
        criteria.EntityIds = new[] { request.EntityId };
        criteria.EntityType = request.EntityType;

        if (!string.IsNullOrEmpty(request.Filter))
        {
            var parseResult = _phraseParser.Parse(request.Filter);

            criteria.Keyword = parseResult.Keyword;

            // Term filters
            foreach (var term in parseResult.Filters.OfType<TermFilter>())
            {
                term.MapTo(criteria);
            }

            // Custom ModifiedDate filter
            parseResult.Filters.Get<RangeFilter>("ModifiedDate").MapTo(x => criteria.StartDate = x, x => criteria.EndDate = x);

            // Custom Rating filter
            parseResult.Filters.Get<RangeFilter>("Rating").MapTo(x => criteria.StartRating = x, x => criteria.EndRating = x);
        }

        return criteria;
    }
}
