using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;

public class RatingType: ExtendableGraphType<RatingEntityDto>
{
    public RatingType()
    {
        Name = "Rating";

        Field(x => x.Value, nullable: false).Description("Average rating");
        Field(x => x.ReviewCount, nullable: false).Description("Total count of customer reviews");
    }
}
