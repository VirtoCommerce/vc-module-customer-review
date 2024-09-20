using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.Models;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;
using VirtoCommerce.StoreModule.Core.Services;
using ReviewSettings = VirtoCommerce.CustomerReviews.Core.ModuleConstants.Settings.General;

namespace VirtoCommerce.CustomerReviews.Data.Services
{
    public class RatingService : IRatingService
    {
        private readonly Func<ICustomerReviewRepository> _repositoryFactory;
        private readonly IEnumerable<IRatingCalculator> _ratingCalculators;
        private readonly IStoreService _storeService;
        private readonly IStoreSearchService _storeSearchService;
        private readonly ISettingsManager _settingsManager;

        public RatingService(
            Func<ICustomerReviewRepository> repositoryFactory,
            IEnumerable<IRatingCalculator> ratingCalculators,
            IStoreService storeService,
            IStoreSearchService storeSearchService,
            ISettingsManager settingsManager)

        {
            _repositoryFactory = repositoryFactory;
            _ratingCalculators = ratingCalculators;
            _storeService = storeService;
            _storeSearchService = storeSearchService;
            _settingsManager = settingsManager;
        }

        public Task CalculateAsync(string storeId)
        {
            return Calculate(storeId, null, ReviewEntityTypes.Product);
        }

        public async Task CalculateAsync(ReviewStatusChangeData[] data)
        {
            if (!data.Any())
            {
                return;
            }

            foreach (var store in data.Where(d => d.OldStatus != d.NewStatus).GroupBy(r => new { r.StoreId, r.EntityType }))
            {
                await Calculate(store.Key.StoreId, store.Select(i => i.EntityId).ToArray(), store.Key.EntityType);
            }
        }

        private async Task Calculate(string storeId, string[] entityIds, string entityType)
        {
            IEnumerable<ReviewRatingCalculateDto> reviews;
            var statuses = new[] { (int)CustomerReviewStatus.Approved };
            using (var repository = _repositoryFactory())
            {
                reviews = await repository.GetCustomerReviewsByStoreProductAsync(storeId, entityIds, entityType, statuses);
            }

            var calculator = await GetCalculatorAsync(storeId);

            var entities = new List<RatingEntity>();
            foreach (var productStore in reviews.GroupBy(r => new { r.EntityId, r.StoreId }))
            {
                var storeReviews = productStore.Select(r => r.Rating).ToArray();
                var storeTotalRating = calculator.Calculate(storeReviews);

                entities.Add(new RatingEntity
                {
                    EntityId = productStore.Key.EntityId,
                    EntityType = entityType,
                    StoreId = productStore.Key.StoreId,
                    Value = storeTotalRating,
                    ReviewCount = storeReviews.Length,
                });
            }

            using (var repository = _repositoryFactory())
            {
                var pkMap = new PrimaryKeyResolvingMap();
                var alreadyExistEntities = await repository.GetAsync(storeId, entityIds, entityType);
                foreach (var rating in entities)
                {
                    var target = alreadyExistEntities
                        .FirstOrDefault(x => x.EntityId == rating.EntityId && x.EntityType == entityType && x.StoreId == rating.StoreId);

                    if (target == null)
                    {
                        repository.Add(rating);
                    }
                    else
                    {
                        rating.Patch(target);
                    }
                }

                foreach (var existing in alreadyExistEntities)
                {
                    if (!entities.Any(e => e.EntityId == existing.EntityId && e.EntityType == entityType && e.StoreId == existing.StoreId))
                    {
                        repository.Delete(existing);
                    }
                }

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
            }
        }

        public async Task<RatingProductDto[]> GetForStoreAsync(string storeId, string[] productIds)
        {
            using (var repository = _repositoryFactory())
            {
                var ratings = await repository.GetAsync(storeId, productIds, ReviewEntityTypes.Product);

                return ratings.Select(x => new RatingProductDto
                {
                    Value = x.Value,
                    ProductId = x.EntityId,
                    ReviewCount = x.ReviewCount,
                }).ToArray();
            }
        }

        public async Task<RatingEntityDto[]> GetForStoreAsync(string storeId, string[] entityIds, string entityType)
        {
            using (var repository = _repositoryFactory())
            {
                var ratings = await repository.GetAsync(storeId, entityIds, entityType);

                return ratings.Select(x => new RatingEntityDto
                {
                    Value = x.Value,
                    EntityId = x.EntityId,
                    EntityType = entityType,
                    ReviewCount = x.ReviewCount,
                }).ToArray();
            }
        }

        public async Task<RatingStoreDto[]> GetForCatalogAsync(string catalogId, string[] productIds)
        {
            var storeSearchCriteria = AbstractTypeFactory<StoreSearchCriteria>.TryCreateInstance();
            storeSearchCriteria.Take = int.MaxValue;

            var storeSearchResult = await _storeSearchService.SearchNoCloneAsync(storeSearchCriteria);

            var result = new List<RatingStoreDto>();

            using (var repository = _repositoryFactory())
            {
                var stores = storeSearchResult.Results;
                if (!string.IsNullOrEmpty(catalogId))
                {
                    stores = stores.Where(s => s.Catalog == catalogId).ToList();
                }

                foreach (var store in stores)
                {
                    var ratings = await repository.GetAsync(store.Id, productIds, ReviewEntityTypes.Product);
                    if (ratings.Any())
                    {
                        result.AddRange(ratings.Select(x => new RatingStoreDto
                        {
                            StoreId = store.Id,
                            StoreName = store.Name,
                            ProductId = x.EntityId,
                            Value = x.Value,
                            ReviewCount = x.ReviewCount,
                        }));
                    }
                }
            }

            return result.ToArray();
        }

        public async Task<RatingEntityStoreDto[]> GetRatingsAsync(string[] entityIds, string entityType)
        {
            var storeSearchCriteria = AbstractTypeFactory<StoreSearchCriteria>.TryCreateInstance();
            storeSearchCriteria.Take = int.MaxValue;

            var storeSearchResult = await _storeSearchService.SearchNoCloneAsync(storeSearchCriteria);

            var result = new List<RatingEntityStoreDto>();

            using (var repository = _repositoryFactory())
            {
                var stores = storeSearchResult.Results;

                var ratings = await repository.GetAsync(null, entityIds, entityType);
                if (ratings.Any())
                {
                    result.AddRange(ratings.Select(x => new RatingEntityStoreDto
                    {
                        StoreId = null,
                        StoreName = null,
                        EntityId = x.EntityId,
                        EntityType = entityType,
                        Value = x.Value,
                        ReviewCount = x.ReviewCount,
                    }));
                }

                foreach (var store in stores)
                {
                    ratings = await repository.GetAsync(store.Id, entityIds, entityType);
                    if (ratings.Any())
                    {
                        result.AddRange(ratings.Select(x => new RatingEntityStoreDto
                        {
                            StoreId = store.Id,
                            StoreName = store.Name,
                            EntityId = x.EntityId,
                            EntityType = entityType,
                            Value = x.Value,
                            ReviewCount = x.ReviewCount,
                        }));
                    }
                }
            }

            return result.ToArray();
        }

        private async Task<IRatingCalculator> GetCalculatorAsync(string storeId)
        {
            var calculatorName = await _settingsManager.GetValueAsync<string>(ReviewSettings.CalculationMethod);

            if (!string.IsNullOrEmpty(storeId))
            {
                var store = await _storeService.GetNoCloneAsync(storeId, StoreResponseGroup.Full.ToString());

                if (store != null)
                {
                    calculatorName = store.Settings.GetValue<string>(ReviewSettings.CalculationMethod);
                }
            }

            if (string.IsNullOrWhiteSpace(calculatorName))
            {
                throw new KeyNotFoundException($"Setting not found: {ReviewSettings.CalculationMethod.Name}");
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
