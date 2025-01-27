using GraphQL.Types;
using VirtoCommerce.CustomerReviews.ExperienceApi.Models;
using VirtoCommerce.Xapi.Core.Schemas;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;

public class CreateReviewResultType : ExtendableGraphType<CreateReviewResult>
{
    public CreateReviewResultType()
    {
        Name = "CreateReviewResult";

        Field(x => x.Id, nullable: true);
        Field(x => x.UserName, nullable: true);

        Field<NonNullGraphType<ListGraphType<NonNullGraphType<ReviewValidationErrorType>>>>("validationErrors")
            .Description("A set of errors in case the review is invalid")
            .Resolve(context => context.Source.ValidationErrors);
    }
}
