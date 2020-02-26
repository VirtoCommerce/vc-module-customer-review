using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.CustomerReviews.Core.Events;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.Handlers;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.CustomerReviews.Data.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;

namespace VirtoCommerce.CustomerReviews.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICustomerReviewRepository, CustomerReviewRepository>();
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.CustomerReviews") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<CustomerReviewsDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddSingleton<Func<ICustomerReviewRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICustomerReviewRepository>());

            serviceCollection.AddTransient<ICustomerReviewSearchService, CustomerReviewSearchService>();
            serviceCollection.AddTransient<ICustomerReviewService, CustomerReviewService>();

            serviceCollection.AddTransient<IRatingCalculator, AverageRatingCalculator>();
            serviceCollection.AddTransient<IRatingCalculator, WilsonRatingCalculator>();
            serviceCollection.AddTransient<IRatingService, RatingService>();

            serviceCollection.AddTransient<ReviewStatusChangedEventHandler>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

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

        private const string ConfigStoreGroupName = "CustomerReviews";
        private const string ConfigModuleId = "VirtoCommerce.CustomerReviews";
        private const string ConfigStoreModuleId = "VirtoCommerce.Store";
        private const string ConfigCalculatorSettingsName = "CustomerReviews.Calculation.Method";
        private const string ConfigCalculatorSettingsTitle = "Rating calculation method";


        
        //TODO
        //public override void PostInitialize()
        //{
        //    base.PostInitialize();

        //    //Registering settings to store module allows to use individual values in each store
        //    var settingManager = _container.Resolve<ISettingsManager>();

        //    var storeSettings = settingManager
        //        .GetModuleSettings(ConfigModuleId)
        //        .ToList();
        //    storeSettings.Add(GetCalculatorStoreSettings());
        //    settingManager.RegisterModuleSettings(ConfigStoreModuleId, storeSettings.ToArray());
        //}

        //private SettingEntry GetCalculatorStoreSettings()
        //{
        //    var defaultCalculator = new AverageRatingCalculator();
        //    var calculatorsNames = _container.ResolveAll<IRatingCalculator>()
        //                                     .Select(x => x.Name)
        //                                     .ToArray();
        //    return new SettingEntry
        //    {
        //        GroupName = ConfigStoreGroupName,
        //        Name = ConfigCalculatorSettingsName,
        //        Title = ConfigCalculatorSettingsTitle,
        //        ValueType = SettingValueType.ShortText,
        //        Value = defaultCalculator.Name,
        //        DefaultValue = defaultCalculator.Name,
        //        AllowedValues = calculatorsNames
        //    };
        //}
    }
}
