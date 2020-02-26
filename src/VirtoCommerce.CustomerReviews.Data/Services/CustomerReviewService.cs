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

        public async Task SaveCustomerReviewsAsync(CustomerReview[] customerReviews)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<CustomerReview>>();

            using (var repository = _repositoryFactory())
            {
                
                var alreadyExistEntities = await repository.GetByIdsAsync(customerReviews.Where(m => !m.IsTransient()).Select(x => x.Id));
                foreach (var customerReview in customerReviews)
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

                await _eventPublisher.Publish(new CustomerReviewChangedEvent(changedEntries));
            }

            ClearCache(customerReviews);
        }

        public async Task DeleteCustomerReviewsAsync(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                var customerReviews = await GetByIdsAsync(ids);

                var changedEntries = customerReviews.Select(x => new GenericChangedEntry<CustomerReview>(x, EntryState.Deleted)).ToArray();

                await repository.DeleteCustomerReviewsAsync(ids);
                await repository.UnitOfWork.CommitAsync();
                await _eventPublisher.Publish(new CustomerReviewChangedEvent(changedEntries));

                ClearCache(customerReviews);
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

            using (var repository = _repositoryFactory())
            {
                var reviewsDb = await repository.GetByIdsAsync(ids);
                var reviews = reviewsDb.Select(x =>
                {
                    var result = new GenericChangedEntry<ReviewStatusChangeData>(new ReviewStatusChangeData()
                    {
                        ProductId = x.ProductId,
                        StoreId = x.StoreId,
                        OldStatus = (CustomerReviewStatus)x.ReviewStatus,
                        NewStatus = status
                    }, EntryState.Modified);
                    x.ReviewStatus = (byte)status;

                    return result;
                });

                await repository.UnitOfWork.CommitAsync();
                await _eventPublisher.Publish(new ReviewStatusChangedEvent(reviews));
            }
        }
        

        protected virtual void ClearCache(IEnumerable<CustomerReview> members)
        {
            CustomerReviewCacheRegion.ExpireRegion();
        }
    }
}
