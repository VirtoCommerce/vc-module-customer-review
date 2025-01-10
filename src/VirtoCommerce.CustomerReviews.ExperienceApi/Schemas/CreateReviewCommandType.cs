using GraphQL.Types;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;

public class CreateReviewCommandType : InputObjectGraphType<CreateReviewCommand>
{
    public CreateReviewCommandType()
    {
        Field(x => x.StoreId);
        Field(x => x.EntityId);
        Field(x => x.EntityType);
        Field(x => x.Review);
        Field(x => x.Rating);
        Field<ListGraphType<StringGraphType>>(nameof(CreateReviewCommand.ImageUrls));
    }
}
