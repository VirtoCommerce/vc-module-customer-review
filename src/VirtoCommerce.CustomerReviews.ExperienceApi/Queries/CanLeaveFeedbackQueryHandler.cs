using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Xapi.Core.Infrastructure;
using static VirtoCommerce.OrdersModule.Core.ModuleConstants;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Queries;

public class CanLeaveFeedbackQueryHandler : IQueryHandler<CanLeaveFeedbackQuery, bool>
{
    private readonly ICustomerReviewSearchService _reviewSearchService;
    private readonly ICustomerOrderSearchService _orderSearchService;

    public CanLeaveFeedbackQueryHandler(
        ICustomerReviewSearchService reviewSearchService,
        ICustomerOrderSearchService orderSearchService)
    {
        _reviewSearchService = reviewSearchService;
        _orderSearchService = orderSearchService;
    }

    public async Task<bool> Handle(CanLeaveFeedbackQuery request, CancellationToken cancellationToken)
    {
        // Users can only leave reviews if they have purchased the product and haven't yet left a review.
        return IsProductReview(request) &&
               !await ReviewExists(request) &&
               await OrderExists(request);
    }

    private static bool IsProductReview(CanLeaveFeedbackQuery request)
    {
        return request.EntityType == ReviewEntityTypes.Product;
    }

    private async Task<bool> ReviewExists(CanLeaveFeedbackQuery request)
    {
        var criteria = AbstractTypeFactory<CustomerReviewSearchCriteria>.TryCreateInstance();

        criteria.UserId = request.UserId;
        criteria.EntityType = request.EntityType;
        criteria.EntityIds = [request.EntityId];
        criteria.StoreId = request.StoreId;
        criteria.Take = 0;

        var searchResult = await _reviewSearchService.SearchAsync(criteria);

        return searchResult.TotalCount > 0;
    }

    private async Task<bool> OrderExists(CanLeaveFeedbackQuery request)
    {
        var criteria = AbstractTypeFactory<CustomerOrderSearchCriteria>.TryCreateInstance();

        criteria.Status = CustomerOrderStatus.Completed;
        criteria.StoreIds = [request.StoreId];
        criteria.CustomerId = request.UserId;
        criteria.ProductId = request.EntityId;
        criteria.Take = 0;

        var orderSearchResult = await _orderSearchService.SearchAsync(criteria);

        return orderSearchResult.TotalCount > 0;
    }
}
