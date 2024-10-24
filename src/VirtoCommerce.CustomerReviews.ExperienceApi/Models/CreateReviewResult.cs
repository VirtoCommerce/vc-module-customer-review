using System.Collections.Generic;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Models;

public class CreateReviewResult
{
    public string Id { get; set; }

    public string UserName { get; set; }

    public IList<ReviewValidationError> ValidationErrors { get; protected set; } = [];
}
