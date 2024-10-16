using VirtoCommerce.CustomerReviews.ExperienceApi.Models;
using VirtoCommerce.CustomerReviews.ExperienceApi.Validators;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Commands;

public class CreateReviewCommand : ICommand<CreateReviewResult>, ICreationValidation
{
    public string StoreId { get; set; }

    public string UserId { get; set; }

    public string EntityId { get; set; }

    public string EntityType { get; set; }

    public string Review { get; set; }

    public int Rating { get; set; }
}
