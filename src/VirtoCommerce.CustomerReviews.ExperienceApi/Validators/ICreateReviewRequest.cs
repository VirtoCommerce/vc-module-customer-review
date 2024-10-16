namespace VirtoCommerce.CustomerReviews.ExperienceApi.Validators;

public interface ICreateReviewRequest
{
    string StoreId { get; set; }

    string EntityId { get; set; }

    string EntityType { get; set; }

    string UserId { get; set; }
}
