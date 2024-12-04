using GraphQL.Server;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CustomerReviews.ExperienceApi.Authorization;
using VirtoCommerce.CustomerReviews.ExperienceApi.Middleware;
using VirtoCommerce.CustomerReviews.ExperienceApi.Validators;
using VirtoCommerce.ProfileExperienceApiModule.Data.Aggregates.Vendor;
using VirtoCommerce.Xapi.Core.Extensions;
using VirtoCommerce.Xapi.Core.Infrastructure;
using VirtoCommerce.Xapi.Core.Pipelines;
using VirtoCommerce.XCatalog.Core.Models;

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

        serviceCollection.AddSingleton<ScopedSchemaFactory<AssemblyMarker>>();

        serviceCollection.AddPipeline<SearchProductResponse>(builder =>
        {
            builder.AddMiddleware(typeof(EvalProductRatingMiddleware));
            builder.AddMiddleware(typeof(EvalProductVendorRatingMiddleware));
        });

        serviceCollection.AddPipeline<VendorAggregate>(builder =>
        {
            builder.AddMiddleware(typeof(EvalVendorRatingMiddleware));
        });

        serviceCollection.AddTransient<ReviewValidator>();
        serviceCollection.AddSingleton<IAuthorizationHandler, CustomerReviewAuthorizationHandler>();

        return serviceCollection;
    }
}
