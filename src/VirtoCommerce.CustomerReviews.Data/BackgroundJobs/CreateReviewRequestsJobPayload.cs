using VirtoCommerce.CustomerReviews.Data.Handlers;

namespace VirtoCommerce.CustomerReviews.Data.BackgroundJobs
{
    /// <summary>
    /// Payload of <see cref="CreateReviewRequestsJobHandler"/>: the orders that reached the review-request state.
    /// </summary>
    public class CreateReviewRequestsJobPayload
    {
        public OrderRequestReviewJobArgument[] JobArguments { get; set; }
    }
}
