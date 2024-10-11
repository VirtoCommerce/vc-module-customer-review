using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;
using VirtoCommerce.CustomerReviews.ExperienceApi.Models;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using static VirtoCommerce.OrdersModule.Core.ModuleConstants;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Validators;

public class ReviewValidator : AbstractValidator<CreateCustomerReviewCommand>
{
    private const int _minRatingValue = 1;
    private const int _maxRatingValue = 5;

    private readonly ICustomerReviewSearchService _reviewSearchService;
    private readonly ICustomerOrderSearchService _orderSearchService;

    public ReviewValidator(ICustomerReviewSearchService reviewSearchService, ICustomerOrderSearchService orderSearchService)
    {
        _reviewSearchService = reviewSearchService;
        _orderSearchService = orderSearchService;

        RuleFor(x => x).CustomAsync(Validate);
    }

    private async Task Validate(
        CreateCustomerReviewCommand command,
        ValidationContext<CreateCustomerReviewCommand> context,
        CancellationToken token = default)
    {
        if (!IsProductReview(command))
        {
            context.AddFailure(new ReviewValidationError(nameof(command.EntityType),
                $"Reviews can only be left for products (EntityType={ReviewEntityTypes.Product}).", "WRONG_VALUE"));
        }

        if (!await OrderExists(command))
        {
            context.AddFailure(new ReviewValidationError(nameof(CustomerReview),
                "Only users who have purchased the product can leave a review.", "PRODUCT_MUST_BE_PURCHASED"));
        }

        if (await ReviewExists(command))
        {
            context.AddFailure(new ReviewValidationError(nameof(CustomerReview),
                "User has already left a review for this product.", "DUPLICATE_REVIEW"));
        }

        NotEmptyString(nameof(command.UserName), command.UserName, context);
        NotEmptyString(nameof(command.EntityName), command.EntityName, context);
        NotEmptyString(nameof(command.Review), command.Review, context);

        if (command.Rating < _minRatingValue || command.Rating > _maxRatingValue)
        {
            context.AddFailure(new ReviewValidationError(nameof(command.Rating),
                $"Rating should be in the range from {_minRatingValue} to {_maxRatingValue}.", "OUT_OF_RANGE"));
        }
    }

    private static void NotEmptyString(string propertyName, string value, ValidationContext<CreateCustomerReviewCommand> context)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
        {
            context.AddFailure(new ReviewValidationError(propertyName, $"Property '{propertyName}' must be filled in.", "EMPTY_FIELD"));
        }
    }

    private static bool IsProductReview(CreateCustomerReviewCommand command)
    {
        return command.EntityType == ReviewEntityTypes.Product;
    }

    private async Task<bool> ReviewExists(CreateCustomerReviewCommand command)
    {
        var criteria = AbstractTypeFactory<CustomerReviewSearchCriteria>.TryCreateInstance();

        criteria.UserId = command.UserId;
        criteria.EntityType = command.EntityType;
        criteria.EntityIds = [command.EntityId];
        criteria.StoreId = command.StoreId;
        criteria.Take = 0;

        var searchResult = await _reviewSearchService.SearchAsync(criteria);

        return searchResult.TotalCount > 0;
    }

    private async Task<bool> OrderExists(CreateCustomerReviewCommand command)
    {
        var criteria = AbstractTypeFactory<CustomerOrderSearchCriteria>.TryCreateInstance();

        criteria.Status = CustomerOrderStatus.Completed;
        criteria.StoreIds = [command.StoreId];
        criteria.CustomerId = command.UserId;
        criteria.ProductId = command.EntityId;
        criteria.Take = 0;

        var orderSearchResult = await _orderSearchService.SearchAsync(criteria);

        return orderSearchResult.TotalCount > 0;
    }
}
