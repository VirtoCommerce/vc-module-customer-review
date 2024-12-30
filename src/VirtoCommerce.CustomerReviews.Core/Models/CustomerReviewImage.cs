using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CustomerReviews.Core.Models;

public class CustomerReviewImage : AssetBase
{
    public CustomerReviewImage() : base(nameof(CustomerReviewImage))
    {
    }

    public string CustomerReviewId { get; set; }
}
