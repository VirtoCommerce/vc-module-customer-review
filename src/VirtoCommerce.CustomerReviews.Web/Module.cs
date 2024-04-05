using System;
using System.Linq;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.CustomerReviews.Core.Events;
using VirtoCommerce.CustomerReviews.Core.Notifications;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.BackgroundJobs;
using VirtoCommerce.CustomerReviews.Data.Handlers;
using VirtoCommerce.CustomerReviews.Data.MySql;
using VirtoCommerce.CustomerReviews.Data.PostgreSql;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.CustomerReviews.Data.Services;
using VirtoCommerce.CustomerReviews.Data.SqlServer;
using VirtoCommerce.CustomerReviews.ExperienceApi.Extensions;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.OrdersModule.Core.Events;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.Platform.Hangfire;
using VirtoCommerce.Platform.Hangfire.Extensions;
using VirtoCommerce.StoreModule.Core.Model;
using OrderSettings = VirtoCommerce.OrdersModule.Core.ModuleConstants.Settings;
using ReviewSettings = VirtoCommerce.CustomerReviews.Core.ModuleConstants.Settings;

namespace VirtoCommerce.CustomerReviews.Web
{
    public class Module : IModule, IHasConfiguration
    {
        private IApplicationBuilder _applicationBuilder;

        public ManifestModuleInfo ModuleInfo { get; set; }
        public IConfiguration Configuration { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<CustomerReviewsDbContext>(options =>
            {
                var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");
                var connectionString = Configuration.GetConnectionString(ModuleInfo.Id) ?? Configuration.GetConnectionString("VirtoCommerce");

                switch (databaseProvider)
                {
                    case "MySql":
                        options.UseMySqlDatabase(connectionString);
                        break;
                    case "PostgreSql":
                        options.UsePostgreSqlDatabase(connectionString);
                        break;
                    default:
                        options.UseSqlServerDatabase(connectionString);
                        break;
                }
            });


            serviceCollection.AddTransient<ICustomerReviewRepository, CustomerReviewRepository>();
            serviceCollection.AddSingleton<Func<ICustomerReviewRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICustomerReviewRepository>());

            serviceCollection.AddTransient<ICustomerReviewService, CustomerReviewService>();
            serviceCollection.AddTransient<ICustomerReviewSearchService, CustomerReviewSearchService>();

            serviceCollection.AddTransient<IRatingCalculator, AverageRatingCalculator>();
            serviceCollection.AddTransient<IRatingCalculator, WilsonRatingCalculator>();
            serviceCollection.AddTransient<IRatingService, RatingService>();
            serviceCollection.AddTransient<IRequestReviewService, RequestReviewService>();

            serviceCollection.AddTransient<ReviewStatusChangedEventHandler>();
            serviceCollection.AddTransient<OrderChangedEventHandler>();

            // GraphQL
            serviceCollection.AddExperienceApi();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _applicationBuilder = appBuilder;

            var settingsManager = appBuilder.ApplicationServices.GetRequiredService<ISettingsManager>();
            var orderStatuses = settingsManager.GetObjectSettingAsync(OrderSettings.General.OrderStatus.Name).GetAwaiter().GetResult().AllowedValues;
            ReviewSettings.General.RequestReviewOrderInState.AllowedValues = orderStatuses;

            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ReviewSettings.AllSettings, ModuleInfo.Id);

            var jobSettings = ReviewSettings.JobSettings.Select(s => s.Name).ToList();
            var storeSettings = settingsRegistrar.AllRegisteredSettings.Where(x => x.ModuleId.EqualsInvariant(ModuleInfo.Id) && !jobSettings.Contains(x.Name)).ToList();
            storeSettings.Add(GetCalculatorStoreSetting());
            settingsRegistrar.RegisterSettingsForType(storeSettings, nameof(Store));

            var permissionsRegistrar = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsRegistrar.RegisterPermissions(ModuleInfo.Id, "CustomerReviews", ModuleConstants.Security.Permissions.AllPermissions);

            var notificationRegistrar = appBuilder.ApplicationServices.GetService<INotificationRegistrar>();
            notificationRegistrar.RegisterNotification<CustomerReviewEmailNotification>();

            var handlerRegistrar = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            handlerRegistrar.RegisterHandler<ReviewStatusChangedEvent>(async (message, _) => await appBuilder.ApplicationServices.GetService<ReviewStatusChangedEventHandler>().Handle(message));
            handlerRegistrar.RegisterHandler<OrderChangedEvent>(async (message, _) => await appBuilder.ApplicationServices.GetService<OrderChangedEventHandler>().Handle(message));

            var recurringJobManager = appBuilder.ApplicationServices.GetService<IRecurringJobManager>();
            recurringJobManager.WatchJobSetting(
               settingsManager,
               new SettingCronJobBuilder()
                   .SetEnablerSetting(ReviewSettings.General.RequestReviewEnableJob)
                   .SetCronSetting(ReviewSettings.General.RequestReviewCronJob)
                   .ToJob<RequestCustomerReviewJob>(x => x.Process())
                   .Build());

            using var serviceScope = appBuilder.ApplicationServices.CreateScope();
            var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<CustomerReviewsDbContext>();
            if (databaseProvider == "SqlServer")
            {
                dbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
            }
            dbContext.Database.Migrate();
        }

        public void Uninstall()
        {
            // Nothing to do here
        }

        private SettingDescriptor GetCalculatorStoreSetting()
        {
            var result = ReviewSettings.General.CalculationMethod;
            result.AllowedValues = _applicationBuilder.ApplicationServices.GetServices<IRatingCalculator>()
                .Select(x => x.Name)
                .ToArray<object>();
            return result;
        }
    }
}
