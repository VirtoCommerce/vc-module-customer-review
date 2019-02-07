using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Events;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.Models;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CustomerReviews.Data.Services
{
    public class CustomerReviewService : ServiceBase, ICustomerReviewService
    {
        private readonly Func<ICustomerReviewRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;

        public CustomerReviewService(Func<ICustomerReviewRepository> repositoryFactory, IEventPublisher eventPublisher)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
        }

        public async Task<IEnumerable<CustomerReview>> GetByIdsAsync(IEnumerable<string> customerReviewsIds)
        {
            using (var repository = _repositoryFactory())
            {
                var reviews = await repository.GetByIdsAsync(customerReviewsIds);

                return reviews.Select(x => x.ToModel(AbstractTypeFactory<CustomerReview>.TryCreateInstance())).ToArray();
            }
        }

        public async Task SaveCustomerReviewsAsync(IEnumerable<CustomerReview> items)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            {
                using (var changeTracker = GetChangeTracker(repository))
                {
                    var alreadyExistEntities = await repository.GetByIdsAsync(items.Where(m => !m.IsTransient()).Select(x => x.Id));
                    foreach (var derivativeContract in items)
                    {
                        var sourceEntity = AbstractTypeFactory<CustomerReviewEntity>.TryCreateInstance().FromModel(derivativeContract, pkMap);
                        var targetEntity = alreadyExistEntities.FirstOrDefault(x => x.Id == sourceEntity.Id);
                        if (targetEntity != null)
                        {
                            changeTracker.Attach(targetEntity);
                            sourceEntity.Patch(targetEntity);
                        }
                        else
                        {
                            repository.Add(sourceEntity);
                        }
                    }

                    CommitChanges(repository);
                    pkMap.ResolvePrimaryKeys();
                }
            }
        }

        public async Task DeleteCustomerReviewsAsync(IEnumerable<string> ids)
        {
            using (var repository = _repositoryFactory())
            {
                await repository.DeleteCustomerReviewsAsync(ids);
                CommitChanges(repository);
            }
        }

        public Task ApproveReviewAsync(IEnumerable<string> customerReviewsIds)
        {
            return ChangeReviewStatusAsync(customerReviewsIds, CustomerReviewStatus.Approved);

        }

        private async Task ChangeReviewStatusAsync(IEnumerable<string> ids, CustomerReviewStatus status)
        {
            if (!ids.Any())
            {
                return;
            }

            List<ReviewStatusChangeData> reviews = new List<ReviewStatusChangeData>();
            using (var repository = _repositoryFactory())
            {
                using (var changeTracker = GetChangeTracker(repository))
                {
                    var reviewsDb = await repository.GetByIdsAsync(ids);
                    foreach (var entity in reviewsDb)
                    {
                        reviews.Add(new ReviewStatusChangeData()
                        {
                            ProductId = entity.ProductId,
                            StoreId = entity.StoreId,
                            OldStatus = (CustomerReviewStatus)entity.ReviewStatus,
                            NewStatus = status
                        });
                        entity.ReviewStatus = (byte)status;
                    }
                }
                CommitChanges(repository);
            }

            await _eventPublisher.Publish(new ReviewStatusChangedEvent(reviews));

        }

        public Task RejectReviewAsync(IEnumerable<string> customerReviewsIds)
        {
            return ChangeReviewStatusAsync(customerReviewsIds, CustomerReviewStatus.Rejected);
        }

        public Task ResetReviewStatusAsync(IEnumerable<string> customerReviewsIds)
        {
            return ChangeReviewStatusAsync(customerReviewsIds, CustomerReviewStatus.New);
        }
    }
}
