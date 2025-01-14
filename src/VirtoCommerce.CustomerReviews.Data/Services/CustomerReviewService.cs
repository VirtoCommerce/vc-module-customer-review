using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.AssetsModule.Core.Assets;
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
        private readonly Func<ICustomerReviewRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly IBlobUrlResolver _blobUrlResolver;

        public CustomerReviewService(
            Func<ICustomerReviewRepository> repositoryFactory,
            IPlatformMemoryCache platformMemoryCache,
            IEventPublisher eventPublisher,
            IBlobUrlResolver blobUrlResolver)
            : base(repositoryFactory, platformMemoryCache, eventPublisher)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
            _blobUrlResolver = blobUrlResolver;
        }

        protected override Task<IList<CustomerReviewEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
        {
            return ((ICustomerReviewRepository)repository).GetByIdsAsync(ids);
        }

        protected override IList<CustomerReview> ProcessModels(IList<CustomerReviewEntity> entities, string responseGroup)
        {
            var reviews = base.ProcessModels(entities, responseGroup);

            if (!reviews.IsNullOrEmpty())
            {
                ResolveImageUrls(reviews);
            }

            return reviews;
        }

        public Task ApproveReviewAsync(IList<string> customerReviewsIds)
        {
            return ChangeReviewStatusAsync(customerReviewsIds, CustomerReviewStatus.Approved);
        }

        public Task RejectReviewAsync(IList<string> customerReviewsIds)
        {
            return ChangeReviewStatusAsync(customerReviewsIds, CustomerReviewStatus.Rejected);
        }

        public Task ResetReviewStatusAsync(IList<string> customerReviewsIds)
        {
            return ChangeReviewStatusAsync(customerReviewsIds, CustomerReviewStatus.New);
        }

        public override async Task DeleteAsync(IList<string> ids, bool softDelete = false)
        {
            if (ids.IsNullOrEmpty())
            {
                return;
            }

            await ChangeReviewStatusAsync(ids, CustomerReviewStatus.New);

            GenericCachingRegion<CustomerReview>.ExpireRegion();

            await base.DeleteAsync(ids, softDelete);
        }


        private async Task ChangeReviewStatusAsync(IList<string> ids, CustomerReviewStatus status)
        {
            if (ids.IsNullOrEmpty())
            {
                return;
            }

            var reviewStatusChanges = new List<ReviewStatusChangeData>();

            using var repository = _repositoryFactory();
            var reviews = await repository.GetByIdsAsync(ids);

            foreach (var customerReviewEntity in reviews)
            {
                reviewStatusChanges.Add(new ReviewStatusChangeData
                {
                    Id = customerReviewEntity.Id,
                    EntityId = customerReviewEntity.EntityId,
                    EntityType = customerReviewEntity.EntityType,
                    StoreId = customerReviewEntity.StoreId,
                    OldStatus = (CustomerReviewStatus)customerReviewEntity.ReviewStatus,
                    NewStatus = status,
                });
                customerReviewEntity.ReviewStatus = (byte)status;
            }

            await repository.UnitOfWork.CommitAsync();

            GenericCachingRegion<CustomerReview>.ExpireRegion();
            GenericSearchCachingRegion<CustomerReview>.ExpireRegion();

            await _eventPublisher.Publish(new ReviewStatusChangedEvent(reviewStatusChanges.Select(x =>
                new GenericChangedEntry<ReviewStatusChangeData>(x, EntryState.Modified))));
        }

        private void ResolveImageUrls(IList<CustomerReview> reviews)
        {
            var images = reviews.Where(x => x.Images != null).SelectMany(x => x.Images);

            foreach (var image in images.Where(x => !string.IsNullOrEmpty(x.Url)))
            {
                image.RelativeUrl = image.RelativeUrl?.EmptyToNull() ?? image.Url;
                image.Url = image.Url.StartsWith("/api") ? image.Url : _blobUrlResolver.GetAbsoluteUrl(image.Url);
            }
        }
    }
}
