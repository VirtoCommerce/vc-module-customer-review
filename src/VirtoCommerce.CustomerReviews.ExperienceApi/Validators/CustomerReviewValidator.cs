using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.ExperienceApi.Commands;
using VirtoCommerce.CustomerReviews.ExperienceApi.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Validators;

public class CustomerReviewValidator : AbstractValidator<CreateCustomerReviewCommand>
{
    private readonly ICustomerReviewSearchService _customerReviewSearchService;

    public CustomerReviewValidator(ICustomerReviewSearchService customerReviewSearchService)
    {
        _customerReviewSearchService = customerReviewSearchService;

        // RuleFor(x => x.UserName).NotEmpty();
        // RuleFor(x => x.EntityName).NotEmpty();
        // RuleFor(x => x.Review).NotEmpty();
        // RuleFor(x => x.Rating).GreaterThanOrEqualTo(1).LessThanOrEqualTo(5);

        RuleFor(x => x).CustomAsync(Validate);
    }

    private async Task Validate(CreateCustomerReviewCommand command, ValidationContext<CreateCustomerReviewCommand> context, CancellationToken token = default)
    {
        var reviewSearchCriteria = AbstractTypeFactory<CustomerReviewSearchCriteria>.TryCreateInstance();
        reviewSearchCriteria.UserId = command.UserId;
        reviewSearchCriteria.EntityType = command.EntityType;
        reviewSearchCriteria.EntityIds = [command.EntityId];
        reviewSearchCriteria.StoreId = command.StoreId;

        var searchResult = await _customerReviewSearchService.SearchAsync(reviewSearchCriteria);

        if (searchResult.Results.Count > 0)
        {
            context.AddFailure(new CustomerReviewValidationError(nameof(CustomerReview), "User has already left a review for this product.", "DUPLICATE_REVIEW")
            {
                FormattedMessagePlaceholderValues = new Dictionary<string, object>
                {
                    ["reviewId"] = searchResult.Results.First().Id
                }
            });
        }

        NotEmptyString(nameof(command.UserName), command.UserName, context);
        NotEmptyString(nameof(command.EntityName), command.EntityName, context);
        NotEmptyString(nameof(command.Review), command.Review, context);

        if (command.Rating < 1 || command.Rating > 5)
        {
            context.AddFailure(new CustomerReviewValidationError(nameof(command.Rating), $"Rating should be in the range from 1 to 5.", "OUT_OF_RANGE")
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

    private static void NotEmptyString(string fieldName, string value, ValidationContext<CreateCustomerReviewCommand> context)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
        {
            context.AddFailure(new CustomerReviewValidationError(fieldName, $"Property '{fieldName}' must be filled in.", "EMPTY_FIELD"));
        }
    }
}
