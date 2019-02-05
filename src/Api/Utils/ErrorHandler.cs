using Newtonsoft.Json;

namespace WebApi.Utils
{
    public class ErrorHandler
    {
        [JsonProperty("error_description")]
        public string Description { get; set; }
    }
}