using AutoMapper;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ProfileExperienceApiModule.Data;
using VirtoCommerce.Xapi.Core.Models;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Mapping;

public class RatingProfile : Profile
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

#pragma warning disable CS0618 // Type or member is obsolete
        CreateMap<CreateCustomerReviewCommand, CustomerReview>();
#pragma warning restore CS0618 // Type or member is obsolete
        CreateMap<CreateReviewCommand, CustomerReview>();
    }
}
