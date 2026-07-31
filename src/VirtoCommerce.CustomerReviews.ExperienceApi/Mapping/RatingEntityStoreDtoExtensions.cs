using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ProfileExperienceApiModule.Data;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Mapping;

public static class RatingEntityStoreDtoExtensions
{
    public static ExpVendorRating ToExpVendorRating(this RatingEntityStoreDto source)
    {
        var result = AbstractTypeFactory<ExpVendorRating>.TryCreateInstance();
        result.StoreId = source.StoreId;
        result.Value = source.Value;
        result.ReviewCount = source.ReviewCount;

        return result;
    }
}
