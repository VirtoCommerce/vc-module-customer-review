using System;
using System.Linq;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.ExperienceApi.Mapping;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XCatalog.Core.Models;
using VirtoCommerce.XCatalog.Core.Queries;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Middleware;

public class EvalProductRatingMiddleware : IAsyncMiddleware<SearchProductResponse>
{
    private readonly IRatingService _ratingService;
    private readonly ICustomerReviewMapper _mapper;

    public EvalProductRatingMiddleware(IRatingService ratingService, ICustomerReviewMapper mapper)
    {
        _ratingService = ratingService;
        _mapper = mapper;
    }

    public virtual async Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        var query = parameter.Query;
        if (query == null)
        {
            throw new OperationCanceledException("Query must be set");
        }

        var responseGroup = EnumUtility.SafeParse(query.GetResponseGroup(), ExpProductResponseGroup.None);
        if (responseGroup.HasFlag(ExpProductResponseGroup.LoadRating) && parameter.Results.Any())
        {
            await LoadProductRaiting(parameter, query);
        }

        await next(parameter);
    }

    protected virtual async Task LoadProductRaiting(SearchProductResponse parameter, SearchProductQuery query)
    {
        var productIds = parameter.Results.Select(product => product.Id).ToArray();
        if (productIds.Length == 0)
        {
            return;
        }

        var ratings = await _ratingService.GetForStoreAsync(query.StoreId, productIds, ReviewEntityTypes.Product);

        if (ratings.Length != 0)
        {
            var ratingByIds = ratings.ToDictionary(x => x.EntityId);

            parameter.Results
                .Apply(product =>
                {
                    if (ratingByIds.TryGetValue(product.Id, out var rating))
                    {
                        product.Rating = _mapper.ToExpRating(rating);
                    }
                });
        }
    }
}
