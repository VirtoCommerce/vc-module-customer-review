using FluentValidation.Results;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Models;

public class CustomerReviewValidationError : ValidationFailure
{
    public CustomerReviewValidationError(string type, string error, string errorCode = null)
        : base(type, error)
    {
        ErrorMessage = error;
        ErrorCode = errorCode;
    }
}
