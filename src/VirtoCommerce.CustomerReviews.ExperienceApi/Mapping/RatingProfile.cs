using AutoMapper;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ProfileExperienceApiModule.Data;

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

        CreateMap<RatingEntityStoreDto, ExpVendorRating>().ConvertUsing((src, _) =>
        {
            var result = AbstractTypeFactory<ExpVendorRating>.TryCreateInstance();
            result.StoreId = src.StoreId;
            result.Value = src.Value;
            result.ReviewCount = src.ReviewCount;
            return result;
        });

        CreateMap<CreateCustomerReviewCommand, CustomerReview>();
    }
}
