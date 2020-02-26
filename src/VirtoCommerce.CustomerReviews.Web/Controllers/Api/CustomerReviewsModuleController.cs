using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Web.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.CustomerReviews.Web.Controllers.Api
{
    [Route("api/customerReviews")]
    [ApiController]
    public class CustomerReviewsModuleController : Controller
    {
        private readonly ICustomerReviewSearchService _customerReviewSearchService;
        private readonly ICustomerReviewService _customerReviewService;
        private readonly IStoreService _storeService;
        private readonly IItemService _itemService;

        public CustomerReviewsModuleController(ICustomerReviewSearchService customerReviewSearchService, 
            ICustomerReviewService customerReviewService, 
            IStoreService storeService, 
            IItemService itemService)
        {
            _customerReviewSearchService = customerReviewSearchService;
            _customerReviewService = customerReviewService;
            _storeService = storeService;
            _itemService = itemService;
        }

        /// <summary>
        /// Return list of reviews with product and store name
        /// </summary>
        [HttpPost]
        [Route("reviewList")]
        [Authorize(ModuleConstants.Security.Permissions.CustomerReviewRead)]
        public async Task<ActionResult<CustomerReviewListItemSearchResult>> GetCustomerReviewsList([FromBody]CustomerReviewSearchCriteria criteria)
        {
            var reviews = await _customerReviewSearchService.SearchCustomerReviewsAsync(criteria);

            var storeIds = reviews.Results
                .Select(r => r.StoreId)
                .Distinct()
                .ToArray();

            var stores = await _storeService.GetByIdsAsync(storeIds);

            var productIds = reviews.Results
                .Select(r => r.ProductId)
                .Distinct()
                .ToArray();

            var products = await _itemService.GetByIdsAsync(productIds, ItemResponseGroup.None.ToString());

            List<CustomerReviewListItem> results = new List<CustomerReviewListItem>();
            foreach (var review in reviews.Results)
            {
                var listItem = new CustomerReviewListItem(review)
                {
                    StoreName = stores.FirstOrDefault(s => s.Id == review.StoreId)?.Name,
                    ProductName = products.FirstOrDefault(p => p.Id == review.ProductId)?.Name
                };

                results.Add(listItem);
            }

            var retVal = new CustomerReviewListItemSearchResult { Results = results, TotalCount = reviews.TotalCount };

            return Ok(retVal);
        }

        /// <summary>
        /// Return product Customer review search results
        /// </summary>
        [HttpPost]
        [Route("search")]
        [Authorize(ModuleConstants.Security.Permissions.CustomerReviewRead)]
        public async Task<ActionResult<CustomerReviewSearchResult>> SearchCustomerReviews([FromBody]CustomerReviewSearchCriteria criteria)
        {
            var reviews = await _customerReviewSearchService.SearchCustomerReviewsAsync(criteria);
            return Ok(reviews);
        }

        /// <summary>
        /// Return productIds from changed reviews
        /// </summary>
        [HttpPost]
        [Route("changes")]
        public async Task<ActionResult<string[]>> GetProductIdsOfModifiedReviews([FromBody]ChangedReviewsQuery query)
        {
            var productIds = await _customerReviewSearchService.GetProductIdsOfModifiedReviewsAsync(query);
            return Ok(productIds);
        }

        /// <summary>
        ///  Accept existing customer review
        /// </summary>
        /// <param name="customerReviewsIds">Customer reviews</param>
        /// <returns></returns>
        [HttpPost]
        [Route("approve")]
        [Authorize(ModuleConstants.Security.Permissions.CustomerReviewUpdate)]
        public async Task<ActionResult> ApproveReview(string[] customerReviewsIds)
        {
            await _customerReviewService.ApproveReviewAsync(customerReviewsIds);
            return NoContent();
        }

        /// <summary>
        ///  Reject existing customer review
        /// </summary>
        /// <param name="customerReviewsIds">Customer reviews</param>
        /// <returns></returns>
        [HttpPost]
        [Route("reject")]
        [Authorize(ModuleConstants.Security.Permissions.CustomerReviewUpdate)]
        public async Task<ActionResult> RejectReview(string[] customerReviewsIds)
        {
            await _customerReviewService.RejectReviewAsync(customerReviewsIds);
            return NoContent();
        }

        /// <summary>
        ///  Set New existing customer review
        /// </summary>
        /// <param name="customerReviewsIds">Customer reviews</param>
        /// <returns></returns>
        [HttpPost]
        [Route("reset")]
        [Authorize(ModuleConstants.Security.Permissions.CustomerReviewUpdate)]
        public async Task<ActionResult> ResetReviewStatus(string[] customerReviewsIds)
        {
            await _customerReviewService.ResetReviewStatusAsync(customerReviewsIds);
            return NoContent();
        }

        /// <summary>
        ///  Create new or update existing customer review
        /// </summary>
        /// <param name="customerReviews">Customer reviews</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.CustomerReviewUpdate)]
        public async Task<ActionResult> Update([FromBody]CustomerReview[] customerReviews)
        {
            await _customerReviewService.SaveCustomerReviewsAsync(customerReviews);
            return NoContent();
        }

        /// <summary>
        /// Delete Customer Reviews by IDs
        /// </summary>
        /// <param name="ids">IDs</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.CustomerReviewDelete)]
        public async Task<ActionResult> Delete([FromQuery] string[] ids)
        {
            await _customerReviewService.DeleteCustomerReviewsAsync(ids);
            return NoContent();
        }
    }
}
