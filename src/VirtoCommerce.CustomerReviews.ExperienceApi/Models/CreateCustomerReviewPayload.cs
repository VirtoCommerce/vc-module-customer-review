using System.Collections.Generic;
using VirtoCommerce.CustomerReviews.Core.Models;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Models;

public class CreateCustomerReviewPayload
{
    public CustomerReview CustomerReview { get; set; }

    public IList<CustomerReviewValidationError> ValidationErrors { get; protected set; } = [];
}
