using System;
using System.Linq;
using AutoMapper;
using GraphQL.Server;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.CustomerReviews.Core.Events;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.BackgroundJobs;
using VirtoCommerce.CustomerReviews.Data.Handlers;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.CustomerReviews.Data.Services;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Hangfire;
using VirtoCommerce.Platform.Hangfire.Extensions;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.CustomerReviews.Core.Notifications;
using VirtoCommerce.CustomerReviews.ExperienceApi;
using VirtoCommerce.CustomerReviews.ExperienceApi.Middleware;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.OrdersModule.Core.Events;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.CustomerReviews.Web
{
    public class Module : IModule
    {
        private IApplicationBuilder _applicationBuilder;
        private const string ConfigStoreModuleId = "VirtoCommerce.Store";
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<CustomerReviewsDbContext>((provider, options) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                options.UseSqlServer(configuration.GetConnectionString(ModuleInfo.Id) ?? configuration.GetConnectionString("VirtoCommerce"));
            });

            serviceCollection.AddTransient<ICustomerReviewRepository, CustomerReviewRepository>();
            serviceCollection.AddSingleton<Func<ICustomerReviewRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICustomerReviewRepository>());

            serviceCollection.AddTransient<ICrudService<CustomerReview>, CustomerReviewService>();
            serviceCollection.AddTransient(x => (ICustomerReviewService)x.GetRequiredService<ICrudService<CustomerReview>>());

            serviceCollection.AddTransient<ISearchService<CustomerReviewSearchCriteria, CustomerReviewSearchResult, CustomerReview>, CustomerReviewSearchService>();
            serviceCollection.AddTransient(x => (ICustomerReviewSearchService)x.GetRequiredService<ISearchService<CustomerReviewSearchCriteria, CustomerReviewSearchResult, CustomerReview>>());

            serviceCollection.AddTransient<IRatingCalculator, AverageRatingCalculator>();
            serviceCollection.AddTransient<IRatingCalculator, WilsonRatingCalculator>();
            serviceCollection.AddTransient<IRatingService, RatingService>();
            serviceCollection.AddTransient<IRequestReviewService, RequestReviewService>();

            serviceCollection.AddTransient<ReviewStatusChangedEventHandler>();
            serviceCollection.AddTransient<OrderChangedEventHandler>();

            // GraphQL
            var assemblyMarker = typeof(AssemblyMarker);
            var graphQlBuilder = new CustomGraphQLBuilder(serviceCollection);
            graphQlBuilder.AddGraphTypes(assemblyMarker);
            serviceCollection.AddMediatR(assemblyMarker);
            serviceCollection.AddAutoMapper(assemblyMarker);
            serviceCollection.AddSchemaBuilders(assemblyMarker);

            serviceCollection.AddPipeline<SearchProductResponse>(builder =>
            {
                builder.AddMiddleware(typeof(EvalVendorRatingMiddleware));
            });
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _applicationBuilder = appBuilder;

            var settingsManager = appBuilder.ApplicationServices.GetRequiredService<ISettingsManager>();
            var order_status = settingsManager.GetObjectSettingAsync(VirtoCommerce.OrdersModule.Core.ModuleConstants.Settings.General.OrderStatus.Name).GetAwaiter().GetResult().AllowedValues;
            ModuleConstants.Settings.AllSettings.First(x => x.Name == ModuleConstants.Settings.General.RequestReviewOrderInState.Name).AllowedValues = order_status;

            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

			var jobsettings = ModuleConstants.Settings.JobSettings.Select(s => s.Name).ToList();
            var storeSettings = settingsRegistrar.AllRegisteredSettings.Where(x => x.ModuleId.EqualsInvariant(ModuleInfo.Id) && !jobsettings.Contains(x.Name)).ToList();
            storeSettings.Add(GetCalculatorStoreSetting());
            settingsRegistrar.RegisterSettingsForType(storeSettings, nameof(Store));
            settingsRegistrar.RegisterSettings(storeSettings, ConfigStoreModuleId);

            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission() { GroupName = "CustomerReviews", Name = x }).ToArray());

            var notificationRegistrar = appBuilder.ApplicationServices.GetService<INotificationRegistrar>();
            notificationRegistrar.RegisterNotification<CustomerReviewEmailNotification>();


            var inProcessBus = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<ReviewStatusChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<ReviewStatusChangedEventHandler>().Handle(message));
            inProcessBus.RegisterHandler<OrderChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<OrderChangedEventHandler>().Handle(message));

            var recurringJobManager = appBuilder.ApplicationServices.GetService<IRecurringJobManager>();
            recurringJobManager.WatchJobSetting(
               settingsManager,
               new SettingCronJobBuilder()
                   .SetEnablerSetting(ModuleConstants.Settings.General.RequestReviewEnableJob)
                   .SetCronSetting(ModuleConstants.Settings.General.RequestReviewCronJob)
                   .ToJob<RequestCustomerReviewJob>(x => x.Process())
                   .Build());

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<CustomerReviewsDbContext>();
                dbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }
        }

        public void Uninstall()
        {
        }

        private SettingDescriptor GetCalculatorStoreSetting()
        {
            var calculatorsNames = _applicationBuilder.ApplicationServices.GetServices<IRatingCalculator>()
                                             .Select(x => x.Name)
                                             .ToArray();
            var result = ModuleConstants.Settings.General.CalculationMethod;
            result.AllowedValues = calculatorsNames;
            return result;
        }
    }
}
