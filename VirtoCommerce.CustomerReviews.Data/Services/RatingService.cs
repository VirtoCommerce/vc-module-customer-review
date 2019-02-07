using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Events;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.RatingCalculators;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.Models;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CustomerReviews.Data.Services
{
    public class RatingService : ServiceBase, IRatingService
    {
        private readonly Func<ICustomerReviewRepository> _repositoryFactory;
        private readonly IEnumerable<IRatingCalculator> _ratingCalculators;
        private readonly IStoreService _storeService;


        public RatingService(Func<ICustomerReviewRepository> repositoryFactory,
            IEnumerable<IRatingCalculator> ratingCalculators,
            IStoreService storeService)
        {
            _repositoryFactory = repositoryFactory;
            _ratingCalculators = ratingCalculators;
            _storeService = storeService;
        }

        public Task CalculateAsync(string storeId)
        {
            return Calculate(storeId, null);
        }

        public async Task CalculateAsync(IEnumerable<ReviewStatusChangeData> data)
        {
            if (!data.Any())
            {
                return;
            }

            foreach (var store in data.Where(d => d.OldStatus != d.NewStatus).GroupBy(r => r.StoreId))
            {
                await Calculate(store.Key, store.Select(i => i.ProductId).ToArray());
            }
        }

        private async Task Calculate(string storeId, string[] productIds)
        {
            IEnumerable<ReviewRatingCalculateDto> reviews;
            var statuses = new byte[] { (byte)CustomerReviewStatus.Approved };
            using (var repository = _repositoryFactory())
            {
                reviews = await repository.GetCustomerReviewsByStoreProductAsync(storeId, productIds, statuses);
            }

            var calculator = GetCalculator(storeId);

            List<RatingEntity> entities = new List<RatingEntity>();
            foreach (var productStore in reviews.GroupBy(r => new { r.ProductId, r.StoreId }))
            {
                decimal rating = calculator.Calculate(productStore.Select(r => r.Rating).ToArray());
                entities.Add(new RatingEntity() { ProductId = productStore.Key.ProductId, StoreId = productStore.Key.StoreId, Value = rating });
            }

            using (var repository = _repositoryFactory())
            {
                var pkMap = new PrimaryKeyResolvingMap();
                using (var changeTracker = GetChangeTracker(repository))
                {
                    var alreadyExistEntities = await repository.GetAsync(storeId, productIds);
                    foreach (var rating in entities)
                    {
                        var target = alreadyExistEntities
                            .FirstOrDefault(x => x.ProductId == rating.ProductId && x.StoreId == rating.StoreId);

                        if (target == null)
                        {
                            repository.Add(rating);
                        }
                        else
                        {
                            changeTracker.Attach(target);
                            rating.Patch(target);
                        }
                    }

                    foreach (var existing in alreadyExistEntities)
                    {
                        if (!entities.Any(e => e.ProductId == existing.ProductId && e.StoreId == existing.StoreId))
                            repository.Delete(existing);
                    }

                    CommitChanges(repository);
                    pkMap.ResolvePrimaryKeys();
                }

            }

        }

        public async Task<RatingProductDto[]> GetForStoreAsync(string storeId, IEnumerable<string> productIds)
        {
            using (var repository = _repositoryFactory())
            {
                var ratings = await repository.GetAsync(storeId, productIds);

                return ratings.Select(x => new RatingProductDto
                {
                    Value = x.Value,
                    ProductId = x.ProductId
                }).ToArray();

            }
        }

        public async Task<RatingStoreDto[]> GetForCatalogAsync(string catalogId, IEnumerable<string> productIds)
        {
            var stores = _storeService.SearchStores(new SearchCriteria { Skip = 0, Take = int.MaxValue }).Stores;
            var result = new List<RatingStoreDto>();

            using (var repository = _repositoryFactory())
            {
                foreach (var store in stores.Where(s => s.Catalog == catalogId))
                {
                    var ratings = await repository.GetAsync(store.Id, productIds);
                    if (ratings.Any())
                    {
                        result.AddRange(ratings.Select(x => new RatingStoreDto
                        {
                            Value = x.Value,
                            StoreId = store.Id,
                            StoreName = store.Name
                        }));
                    }
                }
            }

            return result.ToArray();
        }


        private IRatingCalculator GetCalculator(string storeId)
        {
            var store = _storeService.GetById(storeId);
            if (store == null)
            {
                throw new KeyNotFoundException($"Store not found, storeId: {storeId}");
            }

            var calculatorName = store.Settings.GetSettingValue<string>("CustomerReviews.Calculation.Method", null);
            if (string.IsNullOrWhiteSpace(calculatorName))
            {
                throw new KeyNotFoundException("Store settings not found: CustomerReviews.Calculation.Method");
            }

            var ratingCalculator = _ratingCalculators.FirstOrDefault(c => c.Name == calculatorName);
            if (ratingCalculator == null)
            {
                throw new KeyNotFoundException($"{calculatorName} not found in DI container");
            }

            return ratingCalculator;
        }

    }
}
