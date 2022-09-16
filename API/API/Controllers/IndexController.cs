using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using System.Web.Http.ValueProviders;
using API.Models;
using Hangfire;

namespace API.Controllers
{
    public class IndexController : ApiController {
        private readonly BackgroundJobClient _client = new BackgroundJobClient();
        
        // GET: api/
        [Route("api/")]
        [HttpGet]
        public IHttpActionResult Root()
        {
            return Ok("https://www.youtube.com/watch?v=rEq1Z0bjdwc");
        }
        
        // POST: api/index/
        [Route("api/index/")]
        [HttpPost]
        public IHttpActionResult Index(Document[] data)
        {
            if (ModelState.IsValid)
            {
                foreach (var document in data) {
                    _client.Enqueue(() => Engine.DbDocument.IndexDocument(document.Name, document.Url));
                }
                
                return new ResponseMessageResult(Request.CreateResponse(HttpStatusCode.Accepted, "document queued for indexing"));
            }

            return BadRequest();
        }
    }
}