using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Web.Security;
using VirtoCommerce.Platform.Core.Web.Security;

namespace VirtoCommerce.CustomerReviews.Web.Controllers.Api
{
    [RoutePrefix("api/rating")]
    public class CustomerReviewsModuleRatingController : ApiController
    {
        private readonly IRatingService _ratingService;

        public CustomerReviewsModuleRatingController()
        {
        }

        public CustomerReviewsModuleRatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(RatingStoreDto[]))]
        [CheckPermission(Permission = PredefinedPermissions.RatingRead)]
        public async Task<IHttpActionResult> GetForCatalog([FromUri]string catalogId, [FromUri]string[] productIds)
        {
            var result = new RatingStoreDto[0];

            if (!string.IsNullOrWhiteSpace(catalogId) && productIds.Length > 0)
            {
                result = await _ratingService.GetForCatalogAsync(catalogId, productIds);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("productRatingInStore")]
        [ResponseType(typeof(RatingProductDto[]))]
        [CheckPermission(Permission = PredefinedPermissions.RatingRead)]
        public async Task<IHttpActionResult> GetProductRating([FromUri]string storeId, [FromUri]string[] productIds)
        {
            var result = new RatingProductDto[0];

            if (!string.IsNullOrWhiteSpace(storeId) && productIds.Length > 0)
            {
                result = await _ratingService.GetForStoreAsync(storeId, productIds);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("calculateStore")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = PredefinedPermissions.RatingRecalc)]
        public async Task<IHttpActionResult> CalculateStore([FromUri]string storeId)
        {
            if (!string.IsNullOrWhiteSpace(storeId))
            {
                await _ratingService.CalculateAsync(storeId);
            }

            return Ok();
        }


    }
}