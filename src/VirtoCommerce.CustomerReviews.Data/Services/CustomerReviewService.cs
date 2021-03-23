using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CustomerReviews.Core.Events;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.Caching;
using VirtoCommerce.CustomerReviews.Data.Models;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CustomerReviews.Data.Services
{
    public class CustomerReviewService : ICustomerReviewService
    {
        private readonly Func<ICustomerReviewRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public CustomerReviewService(Func<ICustomerReviewRepository> repositoryFactory, IEventPublisher eventPublisher, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
        }

        public async Task<CustomerReview[]> GetByIdsAsync(string[] customerReviewsIds)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetByIdsAsync), string.Join("-", customerReviewsIds));
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CustomerReviewCacheRegion.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var reviews = await repository.GetByIdsAsync(customerReviewsIds);

                    return reviews.Select(x => x.ToModel(AbstractTypeFactory<CustomerReview>.TryCreateInstance())).ToArray();
                }
            });
        }

        public async Task SaveCustomerReviewsAsync(CustomerReview[] items)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<CustomerReview>>();

            using (var repository = _repositoryFactory())
            {

                var alreadyExistEntities = await repository.GetByIdsAsync(items.Where(m => !m.IsTransient()).Select(x => x.Id));
                foreach (var customerReview in items)
                {
                    var sourceEntity = AbstractTypeFactory<CustomerReviewEntity>.TryCreateInstance().FromModel(customerReview, pkMap);
                    var targetEntity = alreadyExistEntities.FirstOrDefault(x => x.Id == sourceEntity.Id);
                    if (targetEntity != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<CustomerReview>(customerReview, targetEntity.ToModel(AbstractTypeFactory<CustomerReview>.TryCreateInstance()), EntryState.Modified));
                        sourceEntity.Patch(targetEntity);
                    }
                    else
                    {
                        repository.Add(sourceEntity);
                        changedEntries.Add(new GenericChangedEntry<CustomerReview>(customerReview, EntryState.Added));
                    }
                }


                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();

                ClearCache();

                await _eventPublisher.Publish(new CustomerReviewChangedEvent(changedEntries));
            }
        }

        public async Task DeleteCustomerReviewsAsync(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                var customerReviews = await GetByIdsAsync(ids);

                var changedEntries = customerReviews.Select(x => new GenericChangedEntry<CustomerReview>(x, EntryState.Deleted)).ToArray();

                await repository.DeleteCustomerReviewsAsync(ids);
                await repository.UnitOfWork.CommitAsync();

                ClearCache();

                await _eventPublisher.Publish(new CustomerReviewChangedEvent(changedEntries));
            }
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
                var reviews = await repository.GetByIdsAsync(ids);

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

                ClearCache();

                await _eventPublisher.Publish(new ReviewStatusChangedEvent(reviewStatusChanges.Select(x =>
                    new GenericChangedEntry<ReviewStatusChangeData>(x, EntryState.Modified))));
            }
        }


        protected virtual void ClearCache()
        {
            CustomerReviewCacheRegion.ExpireRegion();
        }
    }
}
