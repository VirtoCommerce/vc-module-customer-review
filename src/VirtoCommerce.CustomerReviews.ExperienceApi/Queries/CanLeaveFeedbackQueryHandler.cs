using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Queries;
public class CanLeaveFeedbackQueryHandler : IQueryHandler<CanLeaveFeedbackQuery, bool>
{
    private readonly ICustomerReviewSearchService _customerReviewSearchService;
    private readonly ICustomerOrderSearchService _customerOrderSearchService;

    public CanLeaveFeedbackQueryHandler(ICustomerReviewSearchService customerReviewSearchService, ICustomerOrderSearchService customerOrderSearchService)
    {
        _customerReviewSearchService = customerReviewSearchService;
        _customerOrderSearchService = customerOrderSearchService;
    }

    public async Task<bool> Handle(CanLeaveFeedbackQuery request, CancellationToken cancellationToken)
    {
        if (request.EntityType != "Product")
        {
            return false;
        }

        var orderProductSearchCriteria = GetOrderProductSearchCriteria(request);
        var orderSearchResult = await _customerOrderSearchService.SearchAsync(orderProductSearchCriteria);
        var reviewSearchCriteria = GetReviewSearchCriteria(request);
        var reviewSearchResult = await _customerReviewSearchService.SearchAsync(reviewSearchCriteria);

        // Users can only leave reviews if they have purchased the product and haven't yet left a review.
        return reviewSearchResult.TotalCount <= 0 && orderSearchResult.TotalCount > 0;
    }

    private CustomerReviewSearchCriteria GetReviewSearchCriteria(CanLeaveFeedbackQuery request)
    {
        var reviewSearchCriteria = AbstractTypeFactory<CustomerReviewSearchCriteria>.TryCreateInstance();

        reviewSearchCriteria.UserId = request.UserId;
        reviewSearchCriteria.EntityType = request.EntityType;
        reviewSearchCriteria.EntityIds = [request.EntityId];
        reviewSearchCriteria.StoreId = request.StoreId;
        reviewSearchCriteria.Take = 0;

        return reviewSearchCriteria;
    }

    private CustomerOrderSearchCriteria GetOrderProductSearchCriteria(CanLeaveFeedbackQuery request)
    {
        var orderProductSearchCriteria = AbstractTypeFactory<CustomerOrderSearchCriteria>.TryCreateInstance();

        orderProductSearchCriteria.Status = "Completed";
        orderProductSearchCriteria.StoreIds = [request.StoreId];
        orderProductSearchCriteria.CustomerId = request.UserId;
        orderProductSearchCriteria.ProductId = request.EntityId;
        orderProductSearchCriteria.Take = 0;

        return orderProductSearchCriteria;
    }
}
