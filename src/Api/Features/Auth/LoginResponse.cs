using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebApi.Features.Auth
{
    public class LoginResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}