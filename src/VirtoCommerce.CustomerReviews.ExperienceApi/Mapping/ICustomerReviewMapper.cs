using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;
using VirtoCommerce.ProfileExperienceApiModule.Data;
using VirtoCommerce.Xapi.Core.Models;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Mapping;

public interface ICustomerReviewMapper
{
    ExpRating ToExpRating(RatingEntityDto source);

    ExpVendorRating ToExpVendorRating(RatingEntityStoreDto source);

    CustomerReview ToCustomerReview(CreateReviewCommand source);
}
