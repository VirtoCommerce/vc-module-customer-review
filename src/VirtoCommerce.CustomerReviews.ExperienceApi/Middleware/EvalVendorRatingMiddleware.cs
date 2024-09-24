using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.ProfileExperienceApiModule.Data;
using VirtoCommerce.ProfileExperienceApiModule.Data.Aggregates.Vendor;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Middleware;

public class EvalVendorRatingMiddleware : IAsyncMiddleware<VendorAggregate>
{
    private readonly IMapper _mapper;
    private readonly IRatingService _ratingService;

    public EvalVendorRatingMiddleware(IMapper mapper, IRatingService ratingService)
    {
        _mapper = mapper;
        _ratingService = ratingService;
    }

    public virtual async Task Run(VendorAggregate parameter, Func<VendorAggregate, Task> next)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        var ratings = await _ratingService.GetRatingsAsync(new[] { parameter.Member.Id }, parameter.Member.MemberType);
        parameter.Ratings = ratings.Select(rating => _mapper.Map<ExpVendorRating>(rating)).ToArray();

        await next(parameter);
    }
}
