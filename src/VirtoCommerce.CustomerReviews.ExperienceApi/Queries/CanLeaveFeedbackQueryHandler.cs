using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Models.Search;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Queries;
public class CanLeaveFeedbackQueryHandler : IQueryHandler<CanLeaveFeedbackQuery, bool>
{
    private readonly ICustomerReviewSearchService _customerReviewSearchService;
    private readonly ICustomerOrderProductSearchService _customerOrderProductSearchService;

    public CanLeaveFeedbackQueryHandler(ICustomerReviewSearchService customerReviewSearchService, ICustomerOrderProductSearchService customerOrderProductSearchService)
    {
        _customerReviewSearchService = customerReviewSearchService;
        _customerOrderProductSearchService = customerOrderProductSearchService;
    }

    public async Task<bool> Handle(CanLeaveFeedbackQuery request, CancellationToken cancellationToken)
    {
        var orderProductSearchCriteria = GetOrderProductSearchCriteria(request);
        var orderSearchResult = await _customerOrderProductSearchService.SearchAsync(orderProductSearchCriteria);
        var reviewSearchCriteria = GetReviewSearchCriteria(request);
        var reviewSearchResult = await _customerReviewSearchService.SearchAsync(reviewSearchCriteria);

        // Users can only leave reviews if they have purchased the product and haven't yet left a review.
        return reviewSearchResult.Results.Count <= 0 && orderSearchResult.Results.Count > 0;
    }

    private CustomerReviewSearchCriteria GetReviewSearchCriteria(CanLeaveFeedbackQuery request)
    {
        var reviewSearchCriteria = AbstractTypeFactory<CustomerReviewSearchCriteria>.TryCreateInstance();

        reviewSearchCriteria.UserId = request.UserId;
        reviewSearchCriteria.EntityType = request.EntityType;
        reviewSearchCriteria.EntityIds = [request.EntityId];
        reviewSearchCriteria.StoreId = request.StoreId;

        return reviewSearchCriteria;
    }

    private CustomerOrderProductSearchCriteria GetOrderProductSearchCriteria(CanLeaveFeedbackQuery request)
    {
        var orderProductSearchCriteria = AbstractTypeFactory<CustomerOrderProductSearchCriteria>.TryCreateInstance();

        orderProductSearchCriteria.Status = "Completed";
        orderProductSearchCriteria.StoreIds = [request.StoreId];
        orderProductSearchCriteria.CustomerId = request.UserId;
        orderProductSearchCriteria.ProductId = request.EntityId;
        orderProductSearchCriteria.ProductType = request.EntityType;

        return orderProductSearchCriteria;
    }
}
