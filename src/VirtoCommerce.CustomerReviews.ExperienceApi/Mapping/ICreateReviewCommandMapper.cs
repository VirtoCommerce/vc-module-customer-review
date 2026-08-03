using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Mapping;

public interface ICreateReviewCommandMapper
{
    CustomerReview ToCustomerReview(CreateReviewCommand source);
}
