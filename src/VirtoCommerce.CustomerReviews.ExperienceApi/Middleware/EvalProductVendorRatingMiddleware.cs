using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Xapi.Core.Models;
using VirtoCommerce.XCatalog.Core.Models;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Middleware;

public class EvalProductVendorRatingMiddleware : IAsyncMiddleware<SearchProductResponse>
{
    private readonly IMapper _mapper;
    private readonly IRatingService _ratingService;

    public EvalProductVendorRatingMiddleware(IMapper mapper, IRatingService ratingService)
    {
        _mapper = mapper;
        _ratingService = ratingService;
    }

    public virtual async Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
    {
        if (parameter == null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        var query = parameter.Query;
        if (query == null)
        {
            throw new OperationCanceledException("Query must be set");
        }

        var responseGroup = EnumUtility.SafeParse(query.GetResponseGroup(), ExpProductResponseGroup.None);
        if (responseGroup.HasFlag(ExpProductResponseGroup.LoadRating) && parameter.Results.Any())
        {
            await LoadVendorRaiting(parameter, query);
        }

        await next(parameter);
    }

    protected virtual async Task LoadVendorRaiting(SearchProductResponse parameter, SearchProductQuery query)
    {
        var vendors = parameter.Results.Where(product => product.Vendor != null).Select(product => product.Vendor).ToArray();

        var ratingByIds = new Dictionary<(string, string), RatingEntityDto>();

        foreach (var vendorsByType in vendors.GroupBy(vendor => vendor.Type))
        {
            var vendorType = vendorsByType.Key;
            var vendorIds = vendorsByType.Select(vendor => vendor.Id).Distinct().ToArray();
            var ratings = await _ratingService.GetForStoreAsync(query.StoreId, vendorIds, vendorType);

            foreach (var rating in ratings)
            {
                ratingByIds.Add((rating.EntityId, rating.EntityType), rating);
            }
        }

        if (ratingByIds.Any())
        {
            parameter.Results
                .Where(product => product.Vendor != null)
                .Apply(product =>
                {
                    if (ratingByIds.TryGetValue((product.Vendor.Id, product.Vendor.Type), out var rating))
                    {
                        product.Vendor.Rating = _mapper.Map<ExpRating>(rating);
                    }
                });
        }
    }
}
