using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.CustomerReviews.Core.Notifications;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.CustomerReviews.Data.BackgroundJobs
{
    public class RequestCustomerReviewJob
    {
        private readonly ILogger<RequestCustomerReviewJob> _log;
        private readonly INotificationSender _notificationSender;
        private readonly INotificationSearchService _notificationSearchService;
        private readonly IMemberResolver _memberResolver;
        private readonly ICrudService<Store> _storeService;
        private readonly ICrudService<CustomerOrder> _customerOrderService;
        private readonly Func<ICustomerReviewRepository> _customerReviewRepository;
        private readonly ISettingsManager _settingsManager;
        private readonly IItemService _itemService;

        public RequestCustomerReviewJob(ISettingsManager settingsManager, IItemService pItemService, ICrudService<Store> pStoreService, ICrudService<CustomerOrder> pCustomerOrderService, Func<ICustomerReviewRepository> pCustomerReviewRepository, ILogger<RequestCustomerReviewJob> pLog, INotificationSearchService pNotificationSearchService, INotificationSender pNotificationSender, IMemberResolver pMemberResolver)
        {
            _settingsManager = settingsManager;
            _log = pLog;
            _notificationSearchService = pNotificationSearchService;
            _notificationSender = pNotificationSender;
            _memberResolver = pMemberResolver;
            _storeService = pStoreService;
            _customerOrderService = pCustomerOrderService;
            _customerReviewRepository = pCustomerReviewRepository;
            _itemService = pItemService;
        }


        [DisableConcurrentExecution(10)]
        public async Task Process()
        {
            _log.LogTrace($"Start processing CustomerReviewJob job");
            int countDays;
            int maxRequests;
            GetParameters(out countDays, out maxRequests);

            using (var repository = _customerReviewRepository())
            {
                var query = repository.RequestReview
                    .Where(r =>
                        r.AccessDate == null && r.ModifiedDate < DateTime.Now.AddDays(-countDays) && r.ReviewsRequest < maxRequests
                        && !repository.CustomerReviews.Any(cr => r.StoreId == cr.StoreId && r.EntityId == cr.EntityId && r.EntityType == "Product" && cr.UserId == r.UserId));

                var RequestReviews = query.ToList();

                var items = (await _itemService.GetByIdsAsync(RequestReviews.Select(i => i.EntityId).Distinct().ToArray(), CatalogModule.Core.Model.ItemResponseGroup.ItemInfo.ToString())).ToDictionary(i => i.Id).WithDefaultValue(null);

                List<OrderNotificationJobArgument> ordeMail = new List<OrderNotificationJobArgument>();
                foreach (var RequestReview in RequestReviews)
                {
                    var item = items[RequestReview.EntityId];
                    if (item != null && item.EnableReview.GetValueOrDefault(true))
                    {
                        RequestReview.ModifiedDate = DateTime.Now;
                        RequestReview.ReviewsRequest++;
                        repository.Update(RequestReview);

                        ordeMail.Add(new OrderNotificationJobArgument()
                        {
                            RequestId = RequestReview.Id,
                            EntityId = RequestReview.EntityId,
                            EntityType = RequestReview.EntityType,
                            CustomerId = RequestReview.UserId,
                            CustomerOrderId = RequestReview.CustomerOrderId,
                            StoreId = RequestReview.StoreId,
                            NotificationTypeName = nameof(CustomerReviewEmailNotification)
                        });
                    }
                }
                if (ordeMail.Any())
                {
                    await repository.UnitOfWork.CommitAsync();

                    await TryToSendOrderNotificationsAsync(ordeMail.ToArray());
                }
            }

            _log.LogTrace($"Complete processing CustomerReviewJob job");
        }

        private void GetParameters(out int countDays, out int maxRequests)
        {
            var settings = _settingsManager.GetObjectSettingsAsync(new[] { ModuleConstants.Settings.General.RequestReviewDaysInState.Name, ModuleConstants.Settings.General.RequestReviewMaxRequests.Name }).GetAwaiter().GetResult();

            countDays = settings.GetSettingValue(ModuleConstants.Settings.General.RequestReviewDaysInState.Name, (int)ModuleConstants.Settings.General.RequestReviewDaysInState.DefaultValue);
            maxRequests = settings.GetSettingValue(ModuleConstants.Settings.General.RequestReviewMaxRequests.Name, (int)ModuleConstants.Settings.General.RequestReviewMaxRequests.DefaultValue);
        }

        public virtual async Task TryToSendOrderNotificationsAsync(OrderNotificationJobArgument[] jobArguments)
        {
            var ordersByIdDict = (await _customerOrderService.GetAsync(jobArguments.Select(x => x.CustomerOrderId).Distinct().ToList()))
                                .ToDictionary(x => x.Id)
                                .WithDefaultValue(null);
            foreach (var jobArgument in jobArguments)
            {
                var notification = await _notificationSearchService.GetNotificationAsync(jobArgument.NotificationTypeName, new TenantIdentity(jobArgument.StoreId, nameof(Store)));
                if (notification != null)
                {
                    var order = ordersByIdDict[jobArgument.CustomerOrderId];

                    if (order != null && notification is CustomerReviewEmailNotification orderNotification)
                    {
                        var customer = await _memberResolver.ResolveMemberByIdAsync(jobArgument.CustomerId);

                        orderNotification.Item = order.Items.FirstOrDefault(i => i.ProductId == jobArgument.EntityId);
                        orderNotification.Customer = customer;
                        orderNotification.RequestId = jobArgument.RequestId;
                        orderNotification.LanguageCode = order.LanguageCode;

                        await SetNotificationParametersAsync(notification, order, customer);
                        await _notificationSender.ScheduleSendNotificationAsync(notification);
                    }
                }
            }
        }

        protected virtual async Task SetNotificationParametersAsync(Notification pNotification, CustomerOrder pOrder, Member pCustomer)
        {
            var store = await _storeService.GetByIdAsync(pOrder.StoreId, StoreResponseGroup.StoreInfo.ToString());

            if (pNotification is EmailNotification emailNotification)
            {
                emailNotification.From = store.EmailWithName;
                emailNotification.To = GetOrderRecipientEmail(pOrder, pCustomer);
            }

            // Allow to filter notification log either by customer order or by subscription
            if (string.IsNullOrEmpty(pOrder.SubscriptionId))
            {
                pNotification.TenantIdentity = new TenantIdentity(pOrder.Id, nameof(CustomerOrder));
            }
            else
            {
                pNotification.TenantIdentity = new TenantIdentity(pOrder.SubscriptionId, "Subscription");
            }
        }

        protected virtual string GetOrderRecipientEmail(CustomerOrder order, Member pCustomer)
        {

            var email = GetOrderAddressEmail(order) ?? pCustomer?.Emails?.FirstOrDefault();
            return email;
        }

        protected virtual string GetOrderAddressEmail(CustomerOrder order)
        {
            var email = order.Addresses?.Select(x => x.Email).FirstOrDefault(x => !string.IsNullOrEmpty(x));
            return email;
        }
    }

    public class OrderNotificationJobArgument
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
