using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ProfileExperienceApiModule.Data;
using VirtoCommerce.Xapi.Core.Models;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Mapping;

public class CustomerReviewMapper : ICustomerReviewMapper
{
    public virtual ExpRating ToExpRating(RatingEntityDto source)
    {
        if (source == null)
        {
            return null;
        }

        var result = AbstractTypeFactory<ExpRating>.TryCreateInstance();
        result.Value = source.Value;
        result.ReviewCount = source.ReviewCount;

        return result;
    }

    public virtual ExpVendorRating ToExpVendorRating(RatingEntityStoreDto source)
    {
        if (source == null)
        {
            return null;
        }

        var result = AbstractTypeFactory<ExpVendorRating>.TryCreateInstance();
        result.StoreId = source.StoreId;
        result.Value = source.Value;
        result.ReviewCount = source.ReviewCount;

        return result;
    }

    public virtual CustomerReview ToCustomerReview(CreateReviewCommand source)
    {
        if (source == null)
        {
            return null;
        }

        var result = AbstractTypeFactory<CustomerReview>.TryCreateInstance();
        result.StoreId = source.StoreId;
        result.EntityId = source.EntityId;
        result.EntityType = source.EntityType;
        result.UserId = source.UserId;
        result.Review = source.Review;
        result.Rating = source.Rating;

        return result;
    }
}
