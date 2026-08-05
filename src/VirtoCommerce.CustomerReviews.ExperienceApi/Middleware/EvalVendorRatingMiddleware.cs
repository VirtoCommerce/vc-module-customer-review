using System;
using System.Linq;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.ExperienceApi.Mapping;
using VirtoCommerce.ProfileExperienceApiModule.Data.Aggregates.Vendor;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Middleware;

public class EvalVendorRatingMiddleware : IAsyncMiddleware<VendorAggregate>
{
    private readonly ICustomerReviewMapper _mapper;
    private readonly IRatingService _ratingService;

    public EvalVendorRatingMiddleware(ICustomerReviewMapper mapper, IRatingService ratingService)
    {
        _mapper = mapper;
        _ratingService = ratingService;
    }

    public virtual async Task Run(VendorAggregate parameter, Func<VendorAggregate, Task> next)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        var ratings = await _ratingService.GetRatingsAsync(new[] { parameter.Member.Id }, parameter.Member.MemberType);
        parameter.Ratings = ratings.Select(_mapper.ToExpVendorRating).ToArray();

        await next(parameter);
    }
}
