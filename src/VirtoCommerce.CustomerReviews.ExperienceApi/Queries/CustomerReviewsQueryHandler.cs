using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Models;
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

            foreach (var term in parseResult.Filters.OfType<TermFilter>())
            {
                term.MapTo(criteria);
            }

            // Custom ModifiedDate filter
            var modifiedDateRangeFilter = parseResult.Filters.OfType<RangeFilter>().FirstOrDefault(x => x.FieldName.EqualsInvariant("ModifiedDate"));
            var modifiedDateRange = modifiedDateRangeFilter?.Values?.FirstOrDefault();
            if (modifiedDateRange != null)
            {
                if (DateTime.TryParse(modifiedDateRange.Lower, out var startDate))
                {
                    criteria.StartDate = startDate;
                }

                if (DateTime.TryParse(modifiedDateRange.Upper, out var endDate))
                {
                    criteria.EndDate = endDate;
                }
            }

            // Custom ModifiedDate filter
            var ratingRangeFilter = parseResult.Filters.OfType<RangeFilter>().FirstOrDefault(x => x.FieldName.EqualsInvariant("Rating"));
            var ratingRange = ratingRangeFilter?.Values?.FirstOrDefault();
            if (ratingRange != null)
            {
                if (int.TryParse(ratingRange.Lower, out var startRating))
                {
                    criteria.StartRating = startRating;
                }

                if (int.TryParse(ratingRange.Upper, out var endRating))
                {
                    criteria.EndRating = endRating;
                }
            }
        }

        return criteria;
    }
}
