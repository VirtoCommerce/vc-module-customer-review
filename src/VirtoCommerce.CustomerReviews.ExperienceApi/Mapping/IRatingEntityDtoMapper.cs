using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Xapi.Core.Models;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Mapping;

public interface IRatingEntityDtoMapper
{
    ExpRating ToExpRating(RatingEntityDto source);
}
