using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CustomerReviews.Core;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.CustomerReviews.Web.Controllers.Api
{
    [Route("api/customerReviews")]
    [ApiController]
    public class CustomerReviewsModuleController : Controller
    {
        private readonly ISearchService<CustomerReviewSearchCriteria, CustomerReviewSearchResult, CustomerReview> _customerReviewSearchService;
        private readonly ICrudService<CustomerReview> _customerReviewService;
        private readonly ICrudService<Store> _storeService;
        private readonly IItemService _itemService;
        private readonly IRequestReviewService _requestReviewService;

        public CustomerReviewsModuleController(ISearchService<CustomerReviewSearchCriteria, CustomerReviewSearchResult, CustomerReview> customerReviewSearchService,
            ICrudService<CustomerReview> customerReviewService,
            ICrudService<Store> storeService,
            IItemService itemService,
            IRequestReviewService requestReviewService)
        {
            _customerReviewSearchService = customerReviewSearchService;
            _customerReviewService = customerReviewService;
            _storeService = storeService;
            _itemService = itemService;
            _requestReviewService = requestReviewService;
        }

        /// <summary>
        /// Return list of reviews with product and store name
        /// </summary>
        [HttpPost]
        [Route("reviewList")]
        [Authorize(ModuleConstants.Security.Permissions.CustomerReviewRead)]
        public async Task<ActionResult<CustomerReviewListItemSearchResult>> GetCustomerReviewsList([FromBody] CustomerReviewSearchCriteria criteria)
        {
            var reviews = await _customerReviewSearchService.SearchAsync(criteria);

            var storeIds = reviews.Results
                .Select(r => r.StoreId)
                .Distinct()
                .ToList();

            var stores = await _storeService.GetAsync(storeIds);

            var productIds = reviews.Results
                .Select(r => r.ProductId)
                .Distinct()
                .ToArray();

            var products = await _itemService.GetByIdsAsync(productIds, ItemResponseGroup.None.ToString());

            var results = new List<CustomerReviewListItem>();
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
        public async Task<ActionResult<CustomerReviewSearchResult>> SearchCustomerReviews([FromBody] CustomerReviewSearchCriteria criteria)
        {
            var reviews = await _customerReviewSearchService.SearchAsync(criteria);
            return Ok(reviews);
        }

        /// <summary>
        /// Return productIds from changed reviews
        /// </summary>
        [HttpPost]
        [Route("changes")]
        public async Task<ActionResult<string[]>> GetProductIdsOfModifiedReviews([FromBody] ChangedReviewsQuery query)
        {
            var productIds = await ((ICustomerReviewSearchService)_customerReviewSearchService).GetProductIdsOfModifiedReviewsAsync(query);
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
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> ApproveReview(string[] customerReviewsIds)
        {
            await ((ICustomerReviewService)_customerReviewService).ApproveReviewAsync(customerReviewsIds);
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
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> RejectReview(string[] customerReviewsIds)
        {
            await ((ICustomerReviewService)_customerReviewService).RejectReviewAsync(customerReviewsIds);
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
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> ResetReviewStatus(string[] customerReviewsIds)
        {
            await ((ICustomerReviewService)_customerReviewService).ResetReviewStatusAsync(customerReviewsIds);
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
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Update([FromBody] CustomerReview[] customerReviews)
        {
            await _customerReviewService.SaveChangesAsync(customerReviews);
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
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete([FromQuery] string[] ids)
        {
            await _customerReviewService.DeleteAsync(ids);
            return NoContent();
        }

        [HttpPost]
        [Route("viewedRequestReview")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult<string[]>> ViewedRequestReview(string[] requestId)
        {
            await _requestReviewService.MarkAccessRequest(requestId);
            return NoContent();
        }
    }
}
