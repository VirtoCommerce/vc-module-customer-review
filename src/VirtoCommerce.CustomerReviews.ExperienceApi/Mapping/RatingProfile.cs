using AutoMapper;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Mapping;

public class RatingProfile: Profile
{
    public RatingProfile()
    {
        CreateMap<RatingEntityDto, ExpProductVendorRating>().ConvertUsing((src, _) =>
        {
            var result = AbstractTypeFactory<ExpProductVendorRating>.TryCreateInstance();
            result.Value = src.Value;
            result.ReviewCount = src.ReviewCount;
            return result;
        });
    }
}
