using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CustomerReviews.Core.EventHandlers;
using VirtoCommerce.CustomerReviews.Core.Events;
using VirtoCommerce.CustomerReviews.Core.RatingCalculators;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.CustomerReviews.Data.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.CustomerReviews.Web
{
    public class Module : ModuleBase
    {
        private const string ConfigStoreGroupName = "CustomerReviews";
        private const string ConfigModuleId = "VirtoCommerce.CustomerReviews";
        private const string ConfigStoreModuleId = "VirtoCommerce.Store";
        private const string ConfigCalculatorSettingsName = "CustomerReviews.Calculation.Method";
        private const string ConfigCalculatorSettingsTitle = "Rating calculation method";

        private readonly string _connectionString = ConfigurationHelper.GetConnectionStringValue("VirtoCommerce.CustomerReviews") ?? ConfigurationHelper.GetConnectionStringValue("VirtoCommerce");
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        public override void SetupDatabase()
        {
            //Modify database schema with EF migrations
            using (var context = new CustomerReviewRepository(_connectionString))
            {
                var initializer = new SetupDatabaseInitializer<CustomerReviewRepository, Data.Migrations.Configuration>();
                initializer.InitializeDatabase(context);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            // This method is called for each installed module on the first stage of initialization.

            // Register implementations:
            _container.RegisterType<ICustomerReviewRepository>(new InjectionFactory(c => new CustomerReviewRepository(_connectionString, new EntityPrimaryKeyGeneratorInterceptor(), _container.Resolve<AuditableInterceptor>())));
            _container.RegisterType<ICustomerReviewSearchService, CustomerReviewSearchService>();
            _container.RegisterType<ICustomerReviewService, CustomerReviewService>();

            _container.RegisterType<IRatingCalculator, AverageRatingCalculator>(new AverageRatingCalculator().Name);
            _container.RegisterType<IRatingCalculator, WilsonRatingCalculator>(new WilsonRatingCalculator().Name);
            _container.RegisterType<IEnumerable<IRatingCalculator>>(new InjectionFactory(c => new List<IRatingCalculator>(_container.ResolveAll<IRatingCalculator>())));
            _container.RegisterType<IRatingService, RatingService>();

            //register events
            var eventHandlerRegistrar = _container.Resolve<IHandlerRegistrar>();
            eventHandlerRegistrar.RegisterHandler<ReviewStatusChangedEvent>(async (message, token) => await _container.Resolve<ReviewStatusChangedEventHandler>().Handle(message));

        }

        public override void PostInitialize()
        {
            base.PostInitialize();

            //Registering settings to store module allows to use individual values in each store
            var settingManager = _container.Resolve<ISettingsManager>();

            var storeSettings = settingManager
                .GetModuleSettings(ConfigModuleId)
                .ToList();
            storeSettings.Add(GetCalculatorStoreSettings());
            settingManager.RegisterModuleSettings(ConfigStoreModuleId, storeSettings.ToArray());
        }

        private SettingEntry GetCalculatorStoreSettings()
        {
            var defaultCalculator = new AverageRatingCalculator();
            var calculatorsNames = _container.ResolveAll<IRatingCalculator>()
                                             .Select(x => x.Name)
                                             .ToArray();
            return new SettingEntry
            {
                GroupName = ConfigStoreGroupName,
                Name = ConfigCalculatorSettingsName,
                Title = ConfigCalculatorSettingsTitle,
                ValueType = SettingValueType.ShortText,
                Value = defaultCalculator.Name,
                DefaultValue = defaultCalculator.Name,
                AllowedValues = calculatorsNames
            };
        }
    }
}
