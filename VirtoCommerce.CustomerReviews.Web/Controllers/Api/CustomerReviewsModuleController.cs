using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.CustomerReviews.Core.Models;
using VirtoCommerce.CustomerReviews.Core.Services;
using VirtoCommerce.CustomerReviews.Web.Model;
using VirtoCommerce.CustomerReviews.Web.Security;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Web.Security;

namespace VirtoCommerce.CustomerReviews.Web.Controllers.Api
{
    [RoutePrefix("api/customerReviews")]
    public class CustomerReviewsModuleController : ApiController
    {
        private readonly ICustomerReviewSearchService _customerReviewSearchService;
        private readonly ICustomerReviewService _customerReviewService;
        private readonly IStoreService _storeService;
        private readonly IItemService _itemService;

        public CustomerReviewsModuleController()
        {
        }

        public CustomerReviewsModuleController(ICustomerReviewSearchService customerReviewSearchService, ICustomerReviewService customerReviewService, IStoreService storeService, IItemService itemService)
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
        [ResponseType(typeof(GenericSearchResult<CustomerReviewListItem>))]
        [CheckPermission(Permission = PredefinedPermissions.CustomerReviewRead)]
        public async Task<IHttpActionResult> GetCustomerReviewsList(CustomerReviewSearchCriteria criteria)
        {
            var reviews = await _customerReviewSearchService.SearchCustomerReviewsAsync(criteria);

            var storeIds = reviews.Results
                .Select(r => r.StoreId)
                .Distinct()
                .ToArray();

            var stores = _storeService.GetByIds(storeIds);

            var productIds = reviews.Results
                .Select(r => r.ProductId)
                .Distinct()
                .ToArray();

            var products = _itemService.GetByIds(productIds, Domain.Catalog.Model.ItemResponseGroup.None);

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

            var retVal = new GenericSearchResult<CustomerReviewListItem>() { Results = results, TotalCount = reviews.TotalCount };

            return Ok(retVal);
        }

        /// <summary>
        /// Return product Customer review search results
        /// </summary>
        [HttpPost]
        [Route("search")]
        [ResponseType(typeof(GenericSearchResult<CustomerReview>))]
        [CheckPermission(Permission = PredefinedPermissions.CustomerReviewRead)]
        public async Task<IHttpActionResult> SearchCustomerReviews(CustomerReviewSearchCriteria criteria)
        {
            var reviews = await _customerReviewSearchService.SearchCustomerReviewsAsync(criteria);
            return Ok(reviews);
        }

        /// <summary>
        /// Return productIds from changed reviews
        /// </summary>
        [HttpPost]
        [Route("changes")]
        [ResponseType(typeof(string[]))]
        public async Task<IHttpActionResult> GetProductIdsOfModifiedReviews(ChangedReviewsQuery query)
        {
            var productIds = await _customerReviewSearchService.GetProductIdsOfModifiedReviews(query);
            return Ok(productIds);
        }

        /// <summary>
        ///  Accept existing customer review
        /// </summary>
        /// <param name="customerReviews">Customer reviews</param>
        /// <returns></returns>
        [HttpPost]
        [Route("approve")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = PredefinedPermissions.CustomerReviewUpdate)]
        public async Task<IHttpActionResult> ApproveReview(string[] customerReviewsIds)
        {
            await _customerReviewService.ApproveReviewAsync(customerReviewsIds);
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        ///  Reject existing customer review
        /// </summary>
        /// <param name="customerReviews">Customer reviews</param>
        /// <returns></returns>
        [HttpPost]
        [Route("reject")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = PredefinedPermissions.CustomerReviewUpdate)]
        public async Task<IHttpActionResult> RejectReview(string[] customerReviewsIds)
        {
            await _customerReviewService.RejectReviewAsync(customerReviewsIds);
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        ///  Set New existing customer review
        /// </summary>
        /// <param name="customerReviews">Customer reviews</param>
        /// <returns></returns>
        [HttpPost]
        [Route("reset")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = PredefinedPermissions.CustomerReviewUpdate)]
        public async Task<IHttpActionResult> ResetReviewStatus(string[] customerReviewsIds)
        {
            await _customerReviewService.ResetReviewStatusAsync(customerReviewsIds);
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        ///  Create new or update existing customer review
        /// </summary>
        /// <param name="customerReviews">Customer reviews</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = PredefinedPermissions.CustomerReviewUpdate)]
        public async Task<IHttpActionResult> Update(CustomerReview[] customerReviews)
        {
            await _customerReviewService.SaveCustomerReviewsAsync(customerReviews);
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Delete Customer Reviews by IDs
        /// </summary>
        /// <param name="ids">IDs</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = PredefinedPermissions.CustomerReviewDelete)]
        public async Task<IHttpActionResult> Delete([FromUri] string[] ids)
        {
            await _customerReviewService.DeleteCustomerReviewsAsync(ids);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
