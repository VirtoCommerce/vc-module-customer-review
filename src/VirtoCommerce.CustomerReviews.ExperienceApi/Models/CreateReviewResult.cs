using System.Collections.Generic;
using VirtoCommerce.CustomerReviews.Core.Models;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Models;

public class CreateReviewResult
{
    public CustomerReview Review { get; set; }

    public IList<ReviewValidationError> ValidationErrors { get; protected set; } = [];
}
