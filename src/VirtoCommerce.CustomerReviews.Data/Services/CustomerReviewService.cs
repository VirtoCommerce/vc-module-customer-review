using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Events;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.Models;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.Platform.Caching;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.CustomerReviews.Data.Services
{
    public class CustomerReviewService : CrudService<CustomerReview, CustomerReviewEntity, CustomerReviewChangeEvent, CustomerReviewChangedEvent>, ICustomerReviewService
    {

        public CustomerReviewService(Func<ICustomerReviewRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IEventPublisher eventPublisher) :
            base(repositoryFactory, platformMemoryCache, eventPublisher)
        {
        }

        protected async override Task<IEnumerable<CustomerReviewEntity>> LoadEntities(IRepository repository, IEnumerable<string> ids, string responseGroup)
        {
            return ((ICustomerReviewRepository)repository).GetByIdsAsync(ids).Result;
        }


        public Task ApproveReviewAsync(string[] customerReviewsIds)
        {
            return ChangeReviewStatusAsync(customerReviewsIds, CustomerReviewStatus.Approved);
        }

        public Task RejectReviewAsync(string[] customerReviewsIds)
        {
            return ChangeReviewStatusAsync(customerReviewsIds, CustomerReviewStatus.Rejected);
        }

        public Task ResetReviewStatusAsync(string[] customerReviewsIds)
        {
            return ChangeReviewStatusAsync(customerReviewsIds, CustomerReviewStatus.New);
        }

        private async Task ChangeReviewStatusAsync(string[] ids, CustomerReviewStatus status)
        {
            if (!ids.Any())
            {
                return;
            }

            var reviewStatusChanges = new List<ReviewStatusChangeData>();

            using (var repository = _repositoryFactory())
            {
                var reviews = await ((ICustomerReviewRepository)repository).GetByIdsAsync(ids);

                foreach (var customerReviewEntity in reviews)
                {
                    reviewStatusChanges.Add(new ReviewStatusChangeData
                    {
                        Id = customerReviewEntity.Id,
                        ProductId = customerReviewEntity.ProductId,
                        StoreId = customerReviewEntity.StoreId,
                        OldStatus = (CustomerReviewStatus)customerReviewEntity.ReviewStatus,
                        NewStatus = status
                    });
                    customerReviewEntity.ReviewStatus = (byte)status;
                }

                await repository.UnitOfWork.CommitAsync();

                GenericCachingRegion<CustomerReview>.ExpireRegion();

                await _eventPublisher.Publish(new ReviewStatusChangedEvent(reviewStatusChanges.Select(x =>
                    new GenericChangedEntry<ReviewStatusChangeData>(x, EntryState.Modified))));
            }
        }
    }
}
