using VirtoCommerce.OrdersModule.Core.Model.Search;

namespace VirtoCommerce.CustomerReviews.Core.Models.Search;

public class CustomerOrderProductSearchCriteria : CustomerOrderSearchCriteria
{
    /// <summary>
    /// Search orders with a certain product id
    /// </summary>
    public string ProductId { get; set; }
    /// <summary>
    /// Search orders with a certain product type
    /// </summary>
    public string ProductType { get; set; }
}
