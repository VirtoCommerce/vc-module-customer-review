using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Queries;

public class GetRatingQuery : IQuery<RatingEntityDto>
{
    public string StoreId { get; set; }

    public string EntityId { get; set; }

    public string EntityType { get; set; }
}
