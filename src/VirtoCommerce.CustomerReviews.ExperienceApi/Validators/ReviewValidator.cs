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

public class ReviewValidator : AbstractValidator<CreateReviewCommand>
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
        CreateReviewCommand command,
        ValidationContext<CreateReviewCommand> context,
        CancellationToken token = default)
    {
        if (!IsProductReview(command))
        {
            context.AddFailure(new ReviewValidationError(nameof(command.EntityType),
                $"Reviews can only be left for products (EntityType={ReviewEntityTypes.Product}).", "INVALID_ENTITY_TYPE"));
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

        if (string.IsNullOrEmpty(command.Review) || string.IsNullOrWhiteSpace(command.Review))
        {
            context.AddFailure(new ReviewValidationError(nameof(command.Rating),
                $"Property '{nameof(command.Review)}' must be filled in.", "REVIEW_IS_EMPTY"));
        }

        if (command.Rating < _minRatingValue || command.Rating > _maxRatingValue)
        {
            context.AddFailure(new ReviewValidationError(nameof(command.Rating),
                $"Rating should be in the range from {_minRatingValue} to {_maxRatingValue}.", "INVALID_RATING"));
        }
    }

    public bool IsProductReview(ICreateReviewRequest request)
    {
        return request.EntityType == ReviewEntityTypes.Product;
    }

    public async Task<bool> ReviewExists(ICreateReviewRequest request)
    {
        var criteria = AbstractTypeFactory<CustomerReviewSearchCriteria>.TryCreateInstance();

        criteria.UserId = request.UserId;
        criteria.EntityType = request.EntityType;
        criteria.EntityIds = [request.EntityId];
        criteria.StoreId = request.StoreId;
        criteria.Take = 0;

        var searchResult = await _reviewSearchService.SearchAsync(criteria);

        return searchResult.TotalCount > 0;
    }

    public async Task<bool> OrderExists(ICreateReviewRequest request)
    {
        var criteria = AbstractTypeFactory<CustomerOrderSearchCriteria>.TryCreateInstance();

        criteria.Status = CustomerOrderStatus.Completed;
        criteria.StoreIds = [request.StoreId];
        criteria.CustomerId = request.UserId;
        criteria.ProductId = request.EntityId;
        criteria.Take = 0;

        var orderSearchResult = await _orderSearchService.SearchAsync(criteria);

        return orderSearchResult.TotalCount > 0;
    }
}
