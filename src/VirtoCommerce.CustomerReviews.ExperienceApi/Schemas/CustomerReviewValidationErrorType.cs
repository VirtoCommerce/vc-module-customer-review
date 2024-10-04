using GraphQL.Types;
using VirtoCommerce.CustomerReviews.ExperienceApi.Models;
using VirtoCommerce.XCart.Core.Schemas;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;

public class CustomerReviewValidationErrorType : ObjectGraphType<CustomerReviewValidationError>
{
    public CustomerReviewValidationErrorType()
    {
        Field(x => x.ErrorCode, nullable: true).Description("Error code");
        Field(x => x.ErrorMessage, nullable: true).Description("Error message");
        Field<ListGraphType<ErrorParameterType>>(name: "errorParameters", resolve: x => x.Source.ErrorParameters);
    }
}
