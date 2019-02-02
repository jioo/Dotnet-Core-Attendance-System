using Newtonsoft.Json;

namespace Test.Api.Models
{
    public class LoginResponseModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}