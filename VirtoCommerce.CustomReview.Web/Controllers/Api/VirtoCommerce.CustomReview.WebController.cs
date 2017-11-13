using System.Web.Http;

namespace VirtoCommerce.CustomReview.Web.Controllers.Api
{
    [RoutePrefix("api/VirtoCommerce.CustomReview.Web")]
    public class ManagedModuleController : ApiController
    {
        // GET: api/managedModule
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(new { result = "Hello world!" });
        }
    }
}
