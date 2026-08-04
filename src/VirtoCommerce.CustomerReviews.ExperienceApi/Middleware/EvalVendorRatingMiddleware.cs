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
    private readonly IRatingService _ratingService;
    private readonly ICustomerReviewMapper _mapper;

    public EvalVendorRatingMiddleware(IRatingService ratingService, ICustomerReviewMapper mapper)
    {
        _ratingService = ratingService;
        _mapper = mapper;
    }

    public virtual async Task Run(VendorAggregate parameter, Func<VendorAggregate, Task> next)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        var ratings = await _ratingService.GetRatingsAsync(new[] { parameter.Member.Id }, parameter.Member.MemberType);
        parameter.Ratings = ratings.Select(_mapper.ToExpVendorRating).ToArray();

        await next(parameter);
    }
}
