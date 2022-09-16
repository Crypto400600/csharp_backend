using Newtonsoft.Json;

namespace API.Models
{
    public class Document
    {
        [JsonRequired]
        [JsonProperty("url")]
        public string Url { get; set; }
        
        [JsonRequired]
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}