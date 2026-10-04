using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Data.Handlers;
using VirtoCommerce.Platform.Core.Jobs;

namespace VirtoCommerce.CustomerReviews.Data.BackgroundJobs
{
    /// <summary>
    /// Creates the review requests for the order items collected by <see cref="OrderChangedEventHandler"/>, off the save path.
    /// </summary>
    public class CreateReviewRequestsJobHandler(OrderChangedEventHandler eventHandler)
        : IBackgroundJobHandler<CreateReviewRequestsJobPayload>
    {
        public virtual Task Execute(CreateReviewRequestsJobPayload payload, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return eventHandler.TryToSendOrderNotificationsAsync(payload.JobArguments);
        }
    }
}
