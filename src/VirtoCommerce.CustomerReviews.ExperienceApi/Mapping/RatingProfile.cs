using AutoMapper;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ExperienceApiModule.Core;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Mapping;

public class RatingProfile: Profile
{
    public RatingProfile()
    {
        CreateMap<RatingEntityDto, ExpRating>().ConvertUsing((src, _) =>
        {
            var result = AbstractTypeFactory<ExpRating>.TryCreateInstance();
            result.Value = src.Value;
            result.ReviewCount = src.ReviewCount;
            return result;
        });
    }
}
