namespace VirtoCommerce.CustomerReviews.ExperienceApi.Validators;

public interface ICreationValidation
{
    string StoreId { get; set; }

    string EntityId { get; set; }

    string EntityType { get; set; }

    string UserId { get; set; }
}
