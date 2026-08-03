using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ProfileExperienceApiModule.Data;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Mapping;

public class RatingEntityStoreDtoMapper : IRatingEntityStoreDtoMapper
{
    public ExpVendorRating ToExpVendorRating(RatingEntityStoreDto source)
    {
        var result = AbstractTypeFactory<ExpVendorRating>.TryCreateInstance();
        result.StoreId = source.StoreId;
        result.Value = source.Value;
        result.ReviewCount = source.ReviewCount;

        return result;
    }
}
