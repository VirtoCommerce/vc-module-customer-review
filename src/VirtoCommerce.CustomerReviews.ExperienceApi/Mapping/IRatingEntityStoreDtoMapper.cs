using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.ProfileExperienceApiModule.Data;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Mapping;

public interface IRatingEntityStoreDtoMapper
{
    ExpVendorRating ToExpVendorRating(RatingEntityStoreDto source);
}
