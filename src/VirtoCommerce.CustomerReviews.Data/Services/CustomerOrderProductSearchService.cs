using System;
using System.Linq;
using Microsoft.Extensions.Options;
using VirtoCommerce.CustomerReviews.Core.Models.Search;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.OrdersModule.Data.Model;
using VirtoCommerce.OrdersModule.Data.Repositories;
using VirtoCommerce.OrdersModule.Data.Services;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.CustomerReviews.Data.Services;

public class CustomerOrderProductSearchService : CustomerOrderSearchService
{
    public CustomerOrderProductSearchService(
        Func<IOrderRepository> repositoryFactory,
        IPlatformMemoryCache platformMemoryCache,
        ICustomerOrderService crudService,
        IOptions<CrudOptions> crudOptions)
        : base(repositoryFactory, platformMemoryCache, crudService, crudOptions)
    {
    }

    protected override IQueryable<CustomerOrderEntity> BuildQuery(IRepository repository, CustomerOrderSearchCriteria criteria)
    {
        var query = base.BuildQuery(repository, criteria);

        if (criteria is CustomerOrderProductSearchCriteria customerOrderProductSearchCriteria)
        {
            if (!string.IsNullOrEmpty(customerOrderProductSearchCriteria.ProductId))
            {
                query = query.Where(o => o.Items.Any(i => i.ProductId == customerOrderProductSearchCriteria.ProductId));
            }

            if (!string.IsNullOrEmpty(customerOrderProductSearchCriteria.ProductType))
            {
                query = query.Where(o => o.Items.Any(i => i.ProductType == customerOrderProductSearchCriteria.ProductType));
            }
        }

        return query;
    }
}
