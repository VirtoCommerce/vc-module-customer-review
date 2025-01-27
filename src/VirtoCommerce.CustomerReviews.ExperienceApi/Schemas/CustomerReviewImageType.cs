using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.Xapi.Core.Schemas;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;

public class CustomerReviewImageType : ExtendableGraphType<CustomerReviewImage>
{
    public CustomerReviewImageType()
    {
        Name = "CustomerReviewImage";

        Field(x => x.Id);
        Field(x => x.Url);
        Field(x => x.Name);
    }
}
