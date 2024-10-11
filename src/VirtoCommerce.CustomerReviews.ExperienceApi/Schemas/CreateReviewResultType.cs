using GraphQL.Types;
using VirtoCommerce.CustomerReviews.ExperienceApi.Models;
using VirtoCommerce.Xapi.Core.Schemas;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;

public class CreateReviewResultType : ExtendableGraphType<CreateReviewResult>
{
    public CreateReviewResultType()
    {
        Name = "CreateReviewResult";

        Field(x => x.Review.Id, nullable: true);
        Field(x => x.Review.CreatedDate, nullable: true);
        Field(x => x.Review.ModifiedDate, nullable: true);
        Field(x => x.Review.StoreId);
        Field(x => x.Review.UserId);
        Field(x => x.Review.UserName);
        Field(x => x.Review.EntityId);
        Field(x => x.Review.EntityType);
        Field(x => x.Review.EntityName);
        Field(x => x.Review.Title, nullable: true);
        Field(x => x.Review.Review);
        Field(x => x.Review.Rating);
        Field<CustomerReviewStatusType>("reviewStatus", resolve: context => context.Source.Review.ReviewStatus);

        Field<NonNullGraphType<ListGraphType<NonNullGraphType<ReviewValidationErrorType>>>>("validationErrors",
            "A set of errors in case the review is invalid",
            resolve: context => context.Source.ValidationErrors);
    }
}
