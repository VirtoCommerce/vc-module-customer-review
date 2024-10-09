using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;
using VirtoCommerce.CustomerReviews.ExperienceApi.Models;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Validators;

public class ReviewValidator : AbstractValidator<CreateCustomerReviewCommand>
{
    private const int minRatingValue = 1;
    private const int maxRatingValue = 5;

    private readonly ICustomerReviewSearchService _customerReviewSearchService;
    private readonly ICustomerOrderSearchService _customerOrderSearchService;

    public ReviewValidator(ICustomerReviewSearchService customerReviewSearchService, ICustomerOrderSearchService customerOrderSearchService)
    {
        _customerReviewSearchService = customerReviewSearchService;
        _customerOrderSearchService = customerOrderSearchService;

        RuleFor(x => x).CustomAsync(Validate);
    }

    private async Task Validate(CreateCustomerReviewCommand command, ValidationContext<CreateCustomerReviewCommand> context, CancellationToken token = default)
    {
        if (command.EntityType != "Product")
        {
            context.AddFailure(new ReviewValidationError(nameof(command.EntityType), $"Reviews can only be left for products (EntityType=Product).", "WRONG_VALUE"));
        }

        var orderProductSearchCriteria = GetOrderProductSearchCriteria(command);
        var orderSearchResult = await _customerOrderSearchService.SearchAsync(orderProductSearchCriteria);

        if (orderSearchResult.TotalCount <= 0)
        {
            context.AddFailure(new ReviewValidationError(nameof(CustomerReview), "Only users who have purchased the product can leave a review.", "PRODUCT_MUST_BE_PURCHASED"));
        }

        var reviewSearchCriteria = GetReviewSearchCriteria(command);
        var reviewSearchResult = await _customerReviewSearchService.SearchAsync(reviewSearchCriteria);

        if (reviewSearchResult.TotalCount > 0)
        {
            context.AddFailure(new ReviewValidationError(nameof(CustomerReview), "User has already left a review for this product.", "DUPLICATE_REVIEW"));
        }

        NotEmptyString(nameof(command.UserName), command.UserName, context);
        NotEmptyString(nameof(command.EntityName), command.EntityName, context);
        NotEmptyString(nameof(command.Review), command.Review, context);

        if (command.Rating < minRatingValue || command.Rating > maxRatingValue)
        {
            context.AddFailure(new ReviewValidationError(nameof(command.Rating), $"Rating should be in the range from {minRatingValue} to {maxRatingValue}.", "OUT_OF_RANGE"));
        }
    }

    private void NotEmptyString(string fieldName, string value, ValidationContext<CreateCustomerReviewCommand> context)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
        {
            context.AddFailure(new ReviewValidationError(fieldName, $"Property '{fieldName}' must be filled in.", "EMPTY_FIELD"));
        }
    }

    private CustomerReviewSearchCriteria GetReviewSearchCriteria(CreateCustomerReviewCommand command)
    {
        var reviewSearchCriteria = AbstractTypeFactory<CustomerReviewSearchCriteria>.TryCreateInstance();

        reviewSearchCriteria.UserId = command.UserId;
        reviewSearchCriteria.EntityType = command.EntityType;
        reviewSearchCriteria.EntityIds = [command.EntityId];
        reviewSearchCriteria.StoreId = command.StoreId;
        reviewSearchCriteria.Take = 0;

        return reviewSearchCriteria;
    }

    private CustomerOrderSearchCriteria GetOrderProductSearchCriteria(CreateCustomerReviewCommand command)
    {
        var orderProductSearchCriteria = AbstractTypeFactory<CustomerOrderSearchCriteria>.TryCreateInstance();

        orderProductSearchCriteria.Status = "Completed";
        orderProductSearchCriteria.StoreIds = [command.StoreId];
        orderProductSearchCriteria.CustomerId = command.UserId;
        orderProductSearchCriteria.ProductId = command.EntityId;
        orderProductSearchCriteria.Take = 0;

        return orderProductSearchCriteria;
    }
}
