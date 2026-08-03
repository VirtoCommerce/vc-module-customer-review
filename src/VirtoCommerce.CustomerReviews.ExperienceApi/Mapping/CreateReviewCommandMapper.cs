using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Mapping;

public class CreateReviewCommandMapper : ICreateReviewCommandMapper
{
    public CustomerReview ToCustomerReview(CreateReviewCommand source)
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
