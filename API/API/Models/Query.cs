using Newtonsoft.Json;

namespace API.Models {
    public class Query {
        [JsonRequired]
        [JsonProperty("query")]
        public string QueryString { get; set; }
    }
}