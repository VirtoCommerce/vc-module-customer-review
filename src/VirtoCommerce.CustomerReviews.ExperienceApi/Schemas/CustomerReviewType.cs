using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Xapi.Core.Schemas;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;

public class CustomerReviewType : ExtendableGraphType<CustomerReview>
{
    public CustomerReviewType()
    {
        Name = "CustomerReview";

        Field(x => x.Id);
        Field(x => x.CreatedDate);
        Field(x => x.ModifiedDate, nullable: true);
        Field(x => x.StoreId);
        Field(x => x.UserId);
        Field(x => x.UserName);
        Field(x => x.EntityId);
        Field(x => x.EntityType);
        Field(x => x.EntityName);
        Field(x => x.Title, nullable: true);
        Field(x => x.Review);
        Field(x => x.Rating);
        Field<CustomerReviewStatusType>("reviewStatus").Resolve(context => context.Source.ReviewStatus);
    }
}
