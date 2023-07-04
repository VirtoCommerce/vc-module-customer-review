using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.CustomerReviews.Core.Notifications;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using ReviewSettings = VirtoCommerce.CustomerReviews.Core.ModuleConstants.Settings.General;

namespace VirtoCommerce.CustomerReviews.Data.BackgroundJobs
{
    public class RequestCustomerReviewJob
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IItemService _itemService;
        private readonly IStoreService _storeService;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly Func<ICustomerReviewRepository> _customerReviewRepositoryFactory;
        private readonly ILogger<RequestCustomerReviewJob> _log;
        private readonly INotificationSearchService _notificationSearchService;
        private readonly INotificationSender _notificationSender;
        private readonly IMemberResolver _memberResolver;

        public RequestCustomerReviewJob(
            ISettingsManager settingsManager,
            IItemService itemService,
            IStoreService storeService,
            ICustomerOrderService customerOrderService,
            Func<ICustomerReviewRepository> customerReviewRepositoryFactory,
            ILogger<RequestCustomerReviewJob> log,
            INotificationSearchService notificationSearchService,
            INotificationSender notificationSender,
            IMemberResolver memberResolver)
        {
            _settingsManager = settingsManager;
            _itemService = itemService;
            _storeService = storeService;
            _customerOrderService = customerOrderService;
            _customerReviewRepositoryFactory = customerReviewRepositoryFactory;
            _log = log;
            _notificationSearchService = notificationSearchService;
            _notificationSender = notificationSender;
            _memberResolver = memberResolver;
        }

        [DisableConcurrentExecution(10)]
        public async Task Process()
        {
            _log.LogTrace($"Start processing {nameof(RequestCustomerReviewJob)} job");

            var maxRequests = await _settingsManager.GetValueAsync<int>(ReviewSettings.RequestReviewMaxRequests);
            var daysInState = await _settingsManager.GetValueAsync<int>(ReviewSettings.RequestReviewDaysInState);
            var maxModifiedDate = DateTime.Now.AddDays(-daysInState);

            using (var repository = _customerReviewRepositoryFactory())
            {
                var reviews = await repository.GetReviewsWithEmptyAccessDate(maxModifiedDate, maxRequests);
                var productIds = reviews.Select(x => x.EntityId).Distinct().ToArray();
                var productsById = (await _itemService.GetNoCloneAsync(productIds, ItemResponseGroup.ItemInfo.ToString())).ToDictionary(x => x.Id);
                var notificationParameters = new List<NotificationParameters>();

                foreach (var review in reviews)
                {
                    if (productsById.TryGetValue(review.EntityId, out var product) && (product.EnableReview ?? true))
                    {
                        review.ModifiedDate = DateTime.Now;
                        review.ReviewsRequest++;
                        repository.Update(review);

                        notificationParameters.Add(new NotificationParameters
                        {
                            RequestId = review.Id,
                            EntityId = review.EntityId,
                            EntityType = review.EntityType,
                            CustomerId = review.UserId,
                            CustomerOrderId = review.CustomerOrderId,
                            StoreId = review.StoreId,
                            NotificationTypeName = nameof(CustomerReviewEmailNotification),
                        });
                    }
                }

                if (notificationParameters.Any())
                {
                    await repository.UnitOfWork.CommitAsync();
                    await SendNotificationsAsync(notificationParameters);
                }
            }

            _log.LogTrace($"Complete processing {nameof(RequestCustomerReviewJob)} job");
        }


        protected virtual async Task SendNotificationsAsync(IList<NotificationParameters> notificationParameters)
        {
            var orderIds = notificationParameters.Select(x => x.CustomerOrderId).Distinct().ToList();
            var ordersById = (await _customerOrderService.GetAsync(orderIds)).ToDictionary(x => x.Id);

            foreach (var parameters in notificationParameters)
            {
                if (ordersById.TryGetValue(parameters.CustomerOrderId, out var order))
                {
                    var notification = await _notificationSearchService.GetNotificationAsync(
                        parameters.NotificationTypeName,
                        new TenantIdentity(parameters.StoreId, nameof(Store)))
                        as CustomerReviewEmailNotification;

                    if (notification != null)
                    {
                        var customer = await _memberResolver.ResolveMemberByIdAsync(parameters.CustomerId);

                        notification.Item = order.Items.FirstOrDefault(i => i.ProductId == parameters.EntityId);
                        notification.Customer = customer;
                        notification.RequestId = parameters.RequestId;
                        notification.LanguageCode = order.LanguageCode;

                        await SetNotificationParametersAsync(notification, order, customer);
                        await _notificationSender.ScheduleSendNotificationAsync(notification);
                    }
                }
            }
        }

        protected virtual async Task SetNotificationParametersAsync(CustomerReviewEmailNotification notification, CustomerOrder order, Member customer)
        {
            var store = await _storeService.GetNoCloneAsync(order.StoreId, StoreResponseGroup.StoreInfo.ToString());
            notification.From = store.EmailWithName;
            notification.To = GetOrderRecipientEmail(order, customer);

            // Allow to filter notification log either by customer order or by subscription
            notification.TenantIdentity = string.IsNullOrEmpty(order.SubscriptionId)
                ? new TenantIdentity(order.Id, nameof(CustomerOrder))
                : new TenantIdentity(order.SubscriptionId, "Subscription");
        }

        protected virtual string GetOrderRecipientEmail(CustomerOrder order, Member customer)
        {
            var email = GetOrderAddressEmail(order) ?? customer?.Emails?.FirstOrDefault();
            return email;
        }

        protected virtual string GetOrderAddressEmail(CustomerOrder order)
        {
            var email = order.Addresses?.Select(x => x.Email).FirstOrDefault(x => !string.IsNullOrEmpty(x));
            return email;
        }
    }

    public class NotificationParameters
    {
        public string NotificationTypeName { get; set; }
        public string CustomerId { get; set; }
        public string CustomerOrderId { get; set; }
        public string StoreId { get; set; }
        public string RequestId { get; set; }
        public string EntityId { get; set; }
        public string EntityType { get; set; }
    }
}
