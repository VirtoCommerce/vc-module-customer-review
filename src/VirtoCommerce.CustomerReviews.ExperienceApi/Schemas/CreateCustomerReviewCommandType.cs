using System;
using GraphQL.Types;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;

[Obsolete("Use CreateReviewCommandType instead.")]
public class CreateCustomerReviewCommandType : InputObjectGraphType<CreateCustomerReviewCommand>
{
    public CreateCustomerReviewCommandType()
    {
        Field(x => x.StoreId);
        Field(x => x.UserId);
        Field(x => x.UserName);
        Field(x => x.EntityId);
        Field(x => x.EntityType);
        Field(x => x.EntityName);
        Field(x => x.Title);
        Field(x => x.Review);
        Field(x => x.Rating);
    }
}
