using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Middleware;

public class EvalVendorRatingMiddleware : IAsyncMiddleware<SearchProductResponse>
{
    private readonly IMapper _mapper;
    private readonly IRatingService _ratingService;

    public EvalVendorRatingMiddleware(IMapper mapper, IRatingService ratingService)
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
            var vendors = parameter.Results.Where(product => product.Vendor != null).Select(product => product.Vendor).ToArray();

            var ratings = new List<RatingEntityDto>();

            foreach (var vendorsByType in vendors.GroupBy(vendor => vendor.Type))
            {
                var vendorType = vendorsByType.Key;
                var vendorIds = vendorsByType.Select(vendor => vendor.Id).Distinct().ToArray();
                ratings.AddRange(await _ratingService.GetForStoreAsync(query.StoreId, vendorIds, vendorType));
            }

            if (ratings.Any())
            {
                parameter.Results
                    .Where(product => product.Vendor != null)
                    .Apply(product =>
                        product.Vendor.Rating =
                            _mapper.Map<ExpProductVendorRating>(ratings.FirstOrDefault(rating =>
                                rating.EntityId == product.Vendor.Id && rating.EntityType == product.Vendor.Type)));
            }
        }

        await next(parameter);
    }
}
