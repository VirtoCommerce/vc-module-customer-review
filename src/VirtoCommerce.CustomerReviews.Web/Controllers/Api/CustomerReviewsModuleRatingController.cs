using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Web.Model;

namespace VirtoCommerce.CustomerReviews.Web.Controllers.Api
{
    [Route("api/rating")]
    public class CustomerReviewsModuleRatingController : Controller
    {
        private readonly IRatingService _ratingService;

        public CustomerReviewsModuleRatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        [Route("productRatingInCatalog")]
        [Authorize(ModuleConstants.Security.Permissions.CustomerReviewRatingRead)]
        [Obsolete("Use generic entityRating method")]
        public async Task<ActionResult<RatingStoreDto[]>> GetForCatalog([FromBody] ProductCatalogRatingRequest request)
        {
            var result = Array.Empty<RatingStoreDto>();

            if (!string.IsNullOrWhiteSpace(request.CatalogId) && request.ProductIds.Length > 0)
            {
                result = await _ratingService.GetForCatalogAsync(request.CatalogId, request.ProductIds);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("entityRating")]
        [Authorize(ModuleConstants.Security.Permissions.CustomerReviewRatingRead)]
        public async Task<ActionResult<RatingEntityStoreDto[]>> GetEntityRating([FromBody] EntityRatingRequest request)
        {
            var result = Array.Empty<RatingEntityStoreDto>();

            if (request.EntityIds.Length > 0)
            {
                result = await _ratingService.GetRatingsAsync(request.EntityIds, request.EntityType);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("productRatingInStore")]
        [Authorize(ModuleConstants.Security.Permissions.CustomerReviewRatingRead)]
        public async Task<ActionResult<RatingProductDto[]>> GetProductRating([FromBody] ProductStoreRatingRequest request)
        {
            var result = Array.Empty<RatingProductDto>();

            if (!string.IsNullOrWhiteSpace(request.StoreId) && request.ProductIds.Length > 0)
            {
                result = await _ratingService.GetForStoreAsync(request.StoreId, request.ProductIds);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("calculateStore")]
        [Authorize(ModuleConstants.Security.Permissions.CustomerReviewRatingRecalc)]
        public async Task<ActionResult> CalculateStore([FromQuery] string storeId)
        {
            if (!string.IsNullOrWhiteSpace(storeId))
            {
                await _ratingService.CalculateAsync(storeId);
            }

            return Ok();
        }


    }
}
