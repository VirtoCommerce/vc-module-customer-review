using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.ExperienceApi.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.Xapi.Core.Index;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Queries;

public class CustomerReviewsQueryHandler : IQueryHandler<CustomerReviewsQuery, CustomerReviewSearchResult>
{
    private readonly ICustomerReviewSearchService _customerReviewSearchService;
    private readonly ISearchPhraseParser _phraseParser;

    public CustomerReviewsQueryHandler(
        ICustomerReviewSearchService customerReviewSearchService,
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
        criteria.EntityIds = [request.EntityId];
        criteria.EntityType = request.EntityType;
        // XAPI only operates with approved reviews
        criteria.ReviewStatus = [CustomerReviewStatus.Approved];

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
