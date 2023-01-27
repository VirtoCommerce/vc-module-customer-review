using AutoMapper;
using GraphQL.Server;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CustomerReviews.ExperienceApi.Middleware;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.XDigitalCatalog.Queries;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.ProfileExperienceApiModule.Data.Aggregates.Vendor;

namespace VirtoCommerce.CustomerReviews.ExperienceApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExperienceApi(this IServiceCollection serviceCollection)
    {
        var assemblyMarker = typeof(AssemblyMarker);
        var graphQlBuilder = new CustomGraphQLBuilder(serviceCollection);
        graphQlBuilder.AddGraphTypes(assemblyMarker);
        serviceCollection.AddMediatR(assemblyMarker);
        serviceCollection.AddAutoMapper(assemblyMarker);
        serviceCollection.AddSchemaBuilders(assemblyMarker);
        serviceCollection.AddPipeline<SearchProductResponse>(builder =>
        {
            builder.AddMiddleware(typeof(EvalProductVendorRatingMiddleware));
        });

        serviceCollection.AddPipeline<VendorAggregate>(builder =>
        {
            builder.AddMiddleware(typeof(EvalVendorRatingMiddleware));
        });
        return serviceCollection;
    }
}
