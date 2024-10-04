using GraphQL.Types;
using VirtoCommerce.CustomerReviews.ExperienceApi.Models;
using VirtoCommerce.Xapi.Core.Schemas;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;

public class CustomerReviewType: ExtendableGraphType<CreateCustomerReviewPayload>
{
    public CustomerReviewType()
    {
        Field(x => x.CustomerReview.Id, nullable: true);
        Field(x => x.CustomerReview.CreatedDate, nullable: true);
        Field(x => x.CustomerReview.ModifiedDate, nullable: true);
        Field(x => x.CustomerReview.StoreId);
        Field(x => x.CustomerReview.UserId);
        Field(x => x.CustomerReview.UserName);
        Field(x => x.CustomerReview.EntityId);
        Field(x => x.CustomerReview.EntityType);
        Field(x => x.CustomerReview.EntityName);
        Field(x => x.CustomerReview.Title, nullable: true);
        Field(x => x.CustomerReview.Review);
        Field(x => x.CustomerReview.Rating);
        Field<CustomerReviewStatusType>("reviewStatus", resolve: context => context.Source.CustomerReview.ReviewStatus);
        Field<NonNullGraphType<ListGraphType<NonNullGraphType<CustomerReviewValidationErrorType>>>>("validationErrors",
            "A set of errors in case the review is invalid",
            resolve: context => context.Source.ValidationErrors);
    }
}
