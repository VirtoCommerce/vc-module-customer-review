using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using VirtoCommerce.XCart.Core.Schemas;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Models;

public class CustomerReviewValidationError : ValidationFailure
{
    public CustomerReviewValidationError(string type, string error, string errorCode = null)
        : base(type, error)
    {
        ErrorMessage = error;
        ErrorCode = errorCode;
    }

    public List<ErrorParameter> ErrorParameters =>
        FormattedMessagePlaceholderValues
            ?.Select(kvp =>
            {
                if (kvp.Value is List<string> values)
                {
                    return new ErrorParameter { Key = kvp.Key, Value = string.Join(',', values) };
                }
                else
                {
                    return new ErrorParameter { Key = kvp.Key, Value = kvp.Value.ToString() };
                }
            })
            .ToList();
}
