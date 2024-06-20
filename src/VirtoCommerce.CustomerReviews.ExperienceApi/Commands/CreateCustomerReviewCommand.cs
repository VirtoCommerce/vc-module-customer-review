using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Commands;

public class CreateCustomerReviewCommand : ICommand<CustomerReview>
{
    public string StoreId { get; set; }

    public string UserId { get; set; }

    public string UserName { get; set; }

    public string EntityId { get; set; }

    public string EntityName { get; set; }

    public string EntityType { get; set; }

    public string Title { get; set; }

    public string Review { get; set; }

    public int Rating { get; set; }
}
