using FluentValidation.Results;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Models;

public class ReviewValidationError : ValidationFailure
{
    public ReviewValidationError(string propertyName, string errorMessage, string errorCode)
        : base(propertyName, errorMessage)
    {
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }
}
