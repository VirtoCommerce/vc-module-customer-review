using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.CustomerReviews.Data.Models;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.OrdersModule.Core.Events;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings;
using ReviewSettings = VirtoCommerce.CustomerReviews.Core.ModuleConstants.Settings.General;

namespace VirtoCommerce.CustomerReviews.Data.Handlers
{
    public class OrderChangedEventHandler : IEventHandler<OrderChangedEvent>
    {
        private readonly ISettingsManager _settingsManager;
        private readonly ICustomerOrderService _orderService;
        private readonly Func<ICustomerReviewRepository> _customerReviewRepositoryFactory;

        public OrderChangedEventHandler(
            ISettingsManager settingsManager,
            ICustomerOrderService orderService,
            Func<ICustomerReviewRepository> customerReviewRepositoryFactory)
        {
            _settingsManager = settingsManager;
            _orderService = orderService;
            _customerReviewRepositoryFactory = customerReviewRepositoryFactory;
        }

        public async Task Handle(OrderChangedEvent message)
        {
            if (await _settingsManager.GetValueAsync<bool>(ReviewSettings.RequestReviewEnableJob))
            {
                var jobArguments = message.ChangedEntries.SelectMany(GetJobArgumentsForChangedEntry).ToArray();
                if (jobArguments.Any())
                {
                    BackgroundJob.Enqueue(() => TryToSendOrderNotificationsAsync(jobArguments));
                }
            }
        }

        protected virtual OrderRequestReviewJobArgument[] GetJobArgumentsForChangedEntry(GenericChangedEntry<CustomerOrder> changedEntry)
        {
            var result = new List<OrderRequestReviewJobArgument>();
            var state = _settingsManager.GetValue<string>(ReviewSettings.RequestReviewOrderInState);

            if (IsOrderInState(changedEntry, state))
            {
                result.Add(OrderRequestReviewJobArgument.FromChangedEntry(changedEntry));
            }
            return result.ToArray();
        }

        protected virtual bool IsOrderInState(GenericChangedEntry<CustomerOrder> changedEntry, string state)
        {
            var result = changedEntry.OldEntry.Status != changedEntry.NewEntry.Status && changedEntry.NewEntry.Status.EqualsInvariant(state);
            return result;
        }

        public virtual async Task TryToSendOrderNotificationsAsync(OrderRequestReviewJobArgument[] jobArguments)
        {
            var ordersByIdDict = (await _orderService.GetAsync(jobArguments.Select(x => x.CustomerOrderId).Distinct().ToList()))
                                .ToDictionary(x => x.Id)
                                .WithDefaultValue(null);
            using (var repository = _customerReviewRepositoryFactory())
            {
                foreach (var jobArgument in jobArguments)
                {
                    var order = ordersByIdDict[jobArgument.CustomerOrderId];

                    if (order != null)
                    {
                        foreach (var item in order.Items)
                        {
                            repository.Add(new RequestReviewEntity
                            {
                                CreatedDate = DateTime.Now,
                                CustomerOrderId = jobArgument.CustomerOrderId,
                                ModifiedDate = DateTime.Now,
                                EntityId = item.ProductId,
                                EntityType = ReviewEntityTypes.Product,
                                ReviewsRequest = 0,
                                StoreId = jobArgument.StoreId,
                                UserId = jobArgument.CustomerId
                            });
                        }
                    }
                }
                await repository.UnitOfWork.CommitAsync();
            }
        }
    }

    public class OrderRequestReviewJobArgument
    {
        public string CustomerId { get; set; }
        public string CustomerOrderId { get; set; }
        public string StoreId { get; set; }

        public static OrderRequestReviewJobArgument FromChangedEntry(GenericChangedEntry<CustomerOrder> changedEntry)
        {
            var result = new OrderRequestReviewJobArgument
            {
                CustomerOrderId = changedEntry.NewEntry.Id ?? changedEntry.OldEntry?.Id,
                StoreId = changedEntry.NewEntry.StoreId,
                CustomerId = changedEntry.NewEntry.CustomerId
            };

            return result;
        }
    }
}
