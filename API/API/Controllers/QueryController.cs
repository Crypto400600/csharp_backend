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
using Engine;
using Hangfire;
using Newtonsoft.Json;

namespace API.Controllers {
    public class QueryController : ApiController {
        // POST: api/query
        [Route("api/query")]
        [HttpGet]
        public async Task<IHttpActionResult> Root(string query) {
            var querier = new Querier();
            BaseDocument [] documents = await querier.Search(query);
            JsonConvert.SerializeObject(documents);
            return Ok(documents);
        }
    }
}