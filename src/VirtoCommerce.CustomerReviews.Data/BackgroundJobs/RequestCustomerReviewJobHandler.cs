using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VirtoCommerce.Platform.Core.DistributedLock;
using VirtoCommerce.Platform.Core.Jobs;

namespace VirtoCommerce.CustomerReviews.Data.BackgroundJobs
{
    /// <summary>
    /// Runs <see cref="RequestCustomerReviewJob"/> on the recurring schedule, one run at a time across the worker fleet.
    /// </summary>
    public class RequestCustomerReviewJobHandler(RequestCustomerReviewJob job, IDistributedLock distributedLock, ILogger<RequestCustomerReviewJobHandler> logger)
        : IBackgroundJobHandler<RequestCustomerReviewJobPayload>
    {
        // Replaces Hangfire's [DisableConcurrentExecution(10)]: wait up to 10 seconds for a previous run to finish, then
        // skip this occurrence instead of running a second pass in parallel; the next occurrence picks up the work.
        // Unlike the Hangfire attribute, the lock spans the whole worker fleet, not one Hangfire server.
        private const string LockResource = "customer-reviews:job:request-customer-review";
        private static readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(10);

        public virtual async Task Execute(RequestCustomerReviewJobPayload payload, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            var ran = await distributedLock.TryExecuteAsync(LockResource, _ => job.Process(), _lockTimeout, cancellationToken);
            if (!ran)
            {
                logger.LogInformation("Skipped requesting customer reviews: a previous run still holds the lock.");
            }
        }
    }
}
