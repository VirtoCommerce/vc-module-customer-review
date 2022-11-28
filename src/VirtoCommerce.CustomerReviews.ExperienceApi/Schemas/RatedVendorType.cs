using MediatR;
using VirtoCommerce.CustomerReviews.ExperienceApi.Queries;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Schemas;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Schemas;

public class RatedVendorType: VendorType
{
    public RatedVendorType(IMediator mediator)
    {
        FieldAsync(
            GraphTypeExtenstionHelper.GetActualType<RatingType>(),
            "rating",
            "Vendor rating",
            resolve: async context =>
            {
                var storeId = context.GetArgumentOrValue<string>("storeId");
                var query = AbstractTypeFactory<GetRatingQuery>.TryCreateInstance();
                query.StoreId = storeId;
                query.EntityId = context.Source.Id;
                query.EntityType = context.Source.Type;
                var result = await mediator.Send(query);
                return result;
            });
    }
}
