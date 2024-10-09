using FluentValidation.Results;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Models;

public class ReviewValidationError : ValidationFailure
{
    public ReviewValidationError(string type, string error, string errorCode = null)
        : base(type, error)
    {
        ErrorMessage = error;
        ErrorCode = errorCode;
    }
}
