using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Xapi.Core.Models;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Mapping;

public class RatingEntityDtoMapper : IRatingEntityDtoMapper
{
    public ExpRating ToExpRating(RatingEntityDto source)
    {
        var result = AbstractTypeFactory<ExpRating>.TryCreateInstance();
        result.Value = source.Value;
        result.ReviewCount = source.ReviewCount;

        return result;
    }
}
