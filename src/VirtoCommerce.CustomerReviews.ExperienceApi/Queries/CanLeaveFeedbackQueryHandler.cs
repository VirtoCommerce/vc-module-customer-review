using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.ExperienceApi.Validators;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Queries;

public class CanLeaveFeedbackQueryHandler : IQueryHandler<CanLeaveFeedbackQuery, bool>
{
    private readonly ReviewValidator _reviewValidator;

    public CanLeaveFeedbackQueryHandler(ReviewValidator reviewValidator)
    {
        _reviewValidator = reviewValidator;
    }

    public async Task<bool> Handle(CanLeaveFeedbackQuery request, CancellationToken cancellationToken)
    {
        // Users can only leave reviews if they have purchased the product and haven't yet left a review.
        return _reviewValidator.IsProductReview(request) &&
               await _reviewValidator.OrderExists(request) &&
               !await _reviewValidator.ReviewExists(request);
    }
}
