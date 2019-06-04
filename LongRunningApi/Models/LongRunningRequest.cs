using Newtonsoft.Json;

namespace LongRunningApi.Models
{
    public class LongRunningRequest
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}