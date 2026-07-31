using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Mapping;

public static class CreateReviewCommandExtensions
{
    public static CustomerReview ToCustomerReview(this CreateReviewCommand source)
    {
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
