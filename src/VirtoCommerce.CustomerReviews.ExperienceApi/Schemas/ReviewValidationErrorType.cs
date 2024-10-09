using GraphQL.Types;
using VirtoCommerce.CustomerReviews.ExperienceApi.Models;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;

public class ReviewValidationErrorType : ObjectGraphType<ReviewValidationError>
{
    public ReviewValidationErrorType()
    {
        Field(x => x.ErrorCode, nullable: true).Description("Error code");
        Field(x => x.ErrorMessage, nullable: true).Description("Error message");
    }
}
