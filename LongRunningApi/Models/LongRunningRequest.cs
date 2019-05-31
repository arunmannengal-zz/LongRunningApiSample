using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LongRunningApi.Models
{
    public class LongRunningRequest
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}