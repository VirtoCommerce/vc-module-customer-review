using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Models.Search;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;
using VirtoCommerce.CustomerReviews.ExperienceApi.Models;
using VirtoCommerce.Platform.Core.Common;
using static VirtoCommerce.CustomerReviews.Core.Models.Search.CustomerOrderProductSearchCriteria;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Validators;

public class CustomerReviewValidator : AbstractValidator<CreateCustomerReviewCommand>
{
    private readonly ICustomerReviewSearchService _customerReviewSearchService;
    private readonly ICustomerOrderProductSearchService _customerOrderProductSearchService;

    public CustomerReviewValidator(ICustomerReviewSearchService customerReviewSearchService, ICustomerOrderProductSearchService customerOrderProductSearchService)
    {
        _customerReviewSearchService = customerReviewSearchService;
        _customerOrderProductSearchService = customerOrderProductSearchService;

        RuleFor(x => x).CustomAsync(Validate);
    }

    private async Task Validate(CreateCustomerReviewCommand command, ValidationContext<CreateCustomerReviewCommand> context, CancellationToken token = default)
    {
        var orderProductSearchCriteria = GetOrderProductSearchCriteria(command);
        var orderSearchResult = await _customerOrderProductSearchService.SearchAsync(orderProductSearchCriteria);

        if (orderSearchResult.Results.Count <= 0)
        {
            context.AddFailure(new CustomerReviewValidationError(nameof(CustomerReview), "Only users who have purchased the product can leave a review.", "PRODUCT_MUST_BE_PURCHASED"));
        }

        var reviewSearchCriteria = GetReviewSearchCriteria(command);
        var reviewSearchResult = await _customerReviewSearchService.SearchAsync(reviewSearchCriteria);

        if (reviewSearchResult.Results.Count > 0)
        {
            context.AddFailure(new CustomerReviewValidationError(nameof(CustomerReview), "User has already left a review for this product.", "DUPLICATE_REVIEW")
            {
                FormattedMessagePlaceholderValues = new Dictionary<string, object>
                {
                    ["reviewId"] = reviewSearchResult.Results.First().Id
                }
            });
        }

        NotEmptyString(nameof(command.UserName), command.UserName, context);
        NotEmptyString(nameof(command.EntityName), command.EntityName, context);
        NotEmptyString(nameof(command.Review), command.Review, context);

        if (command.Rating < 1 || command.Rating > 5)
        {
            context.AddFailure(new CustomerReviewValidationError(nameof(command.Rating), "Rating should be in the range from 1 to 5.", "OUT_OF_RANGE")
            {
                FormattedMessagePlaceholderValues = new Dictionary<string, object>
                {
                    ["rating"] = command.Rating,
                    ["minRating"] = 1,
                    ["maxRating"] = 5,
                }
            });
        }
    }

    private void NotEmptyString(string fieldName, string value, ValidationContext<CreateCustomerReviewCommand> context)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
        {
            context.AddFailure(new CustomerReviewValidationError(fieldName, $"Property '{fieldName}' must be filled in.", "EMPTY_FIELD"));
        }
    }

    private CustomerReviewSearchCriteria GetReviewSearchCriteria(CreateCustomerReviewCommand command)
    {
        var reviewSearchCriteria = AbstractTypeFactory<CustomerReviewSearchCriteria>.TryCreateInstance();

        reviewSearchCriteria.UserId = command.UserId;
        reviewSearchCriteria.EntityType = command.EntityType;
        reviewSearchCriteria.EntityIds = [command.EntityId];
        reviewSearchCriteria.StoreId = command.StoreId;

        return reviewSearchCriteria;
    }

    private CustomerOrderProductSearchCriteria GetOrderProductSearchCriteria(CreateCustomerReviewCommand command)
    {
        var orderProductSearchCriteria = AbstractTypeFactory<CustomerOrderProductSearchCriteria>.TryCreateInstance();

        orderProductSearchCriteria.Status = "Completed";
        orderProductSearchCriteria.StoreIds = [command.StoreId];
        orderProductSearchCriteria.CustomerId = command.UserId;
        orderProductSearchCriteria.ProductId = command.EntityId;
        orderProductSearchCriteria.ProductType = command.EntityType;

        return orderProductSearchCriteria;
    }
}
