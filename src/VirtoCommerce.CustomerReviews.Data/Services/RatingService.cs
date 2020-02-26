using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Data.Models;
using VirtoCommerce.CustomerReviews.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Model.Search;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.CustomerReviews.Data.Services
{
    public class RatingService : IRatingService
    {
        private readonly Func<ICustomerReviewRepository> _repositoryFactory;
        private readonly IEnumerable<IRatingCalculator> _ratingCalculators;
        private readonly IStoreService _storeService;
        private readonly IStoreSearchService _storeSearchService;


        public RatingService(Func<ICustomerReviewRepository> repositoryFactory,
            IEnumerable<IRatingCalculator> ratingCalculators,
            IStoreService storeService, IStoreSearchService storeSearchService)
        {
            _repositoryFactory = repositoryFactory;
            _ratingCalculators = ratingCalculators;
            _storeService = storeService;
            _storeSearchService = storeSearchService;
        }

        public Task CalculateAsync(string storeId)
        {
            return Calculate(storeId, null);
        }

        public async Task CalculateAsync(ReviewStatusChangeData[] data)
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
            var statuses = new [] { (int)CustomerReviewStatus.Approved };
            using (var repository = _repositoryFactory())
            {
                reviews = await repository.GetCustomerReviewsByStoreProductAsync(storeId, productIds, statuses);
            }

            var calculator = await GetCalculatorAsync(storeId);

            List<RatingEntity> entities = new List<RatingEntity>();
            foreach (var productStore in reviews.GroupBy(r => new { r.ProductId, r.StoreId }))
            {
                decimal rating = calculator.Calculate(productStore.Select(r => r.Rating).ToArray());
                entities.Add(new RatingEntity() { ProductId = productStore.Key.ProductId, StoreId = productStore.Key.StoreId, Value = rating });
            }

            using (var repository = _repositoryFactory())
            {
                var pkMap = new PrimaryKeyResolvingMap();
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
                        rating.Patch(target);
                    }
                }

                foreach (var existing in alreadyExistEntities)
                {
                    if (!entities.Any(e => e.ProductId == existing.ProductId && e.StoreId == existing.StoreId))
                        repository.Delete(existing);
                }

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();

            }

        }

        public async Task<RatingProductDto[]> GetForStoreAsync(string storeId, string[] productIds)
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

        public async Task<RatingStoreDto[]> GetForCatalogAsync(string catalogId, string[] productIds)
        {
            var storeSearchCriteria = AbstractTypeFactory<StoreSearchCriteria>.TryCreateInstance();
            storeSearchCriteria.Take = int.MaxValue;
            var storeSearchResult = await _storeSearchService.SearchStoresAsync(storeSearchCriteria);
            var result = new List<RatingStoreDto>();

            using (var repository = _repositoryFactory())
            {
                foreach (var store in storeSearchResult.Results.Where(s => s.Catalog == catalogId))
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


        private async Task<IRatingCalculator> GetCalculatorAsync(string storeId)
        {
            var store = await _storeService.GetByIdAsync(storeId);
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
