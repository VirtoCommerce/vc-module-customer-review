using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.CustomerReviews.Core.Events;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.Handlers;
using VirtoCommerce.CustomerReviews.Data.Models;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.CustomerReviews.Data.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.CustomerReviews.Web
{
    public class Module : IModule
    {
        private IApplicationBuilder _applicationBuilder;
        private const string ConfigStoreModuleId = "VirtoCommerce.Store";
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICustomerReviewRepository, CustomerReviewRepository>();
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.CustomerReviews") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<CustomerReviewsDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddSingleton<Func<ICustomerReviewRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICustomerReviewRepository>());

            serviceCollection.AddTransient<ICrudService<CustomerReview>, CustomerReviewService>();
            serviceCollection.AddTransient(x => (ICustomerReviewService)x.GetRequiredService<ICrudService<CustomerReview>>());

            serviceCollection.AddTransient<ISearchService<CustomerReviewSearchCriteria, CustomerReviewSearchResult, CustomerReview>, CustomerReviewSearchService>();
            serviceCollection.AddTransient(x => (ICustomerReviewSearchService)x.GetRequiredService<ISearchService<CustomerReviewSearchCriteria, CustomerReviewSearchResult, CustomerReview>>());

            serviceCollection.AddTransient<IRatingCalculator, AverageRatingCalculator>();
            serviceCollection.AddTransient<IRatingCalculator, WilsonRatingCalculator>();
            serviceCollection.AddTransient<IRatingService, RatingService>();

            serviceCollection.AddTransient<ReviewStatusChangedEventHandler>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _applicationBuilder = appBuilder;
            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

            var storeSettings = settingsRegistrar.AllRegisteredSettings.Where(x => x.ModuleId.EqualsInvariant(ModuleInfo.Id)).ToList();
            storeSettings.Add(GetCalculatorStoreSetting());
            settingsRegistrar.RegisterSettingsForType(storeSettings, nameof(Store));
            settingsRegistrar.RegisterSettings(storeSettings, ConfigStoreModuleId);

            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission() { GroupName = "CustomerReviews", Name = x }).ToArray());

            var inProcessBus = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<ReviewStatusChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<ReviewStatusChangedEventHandler>().Handle(message));

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
