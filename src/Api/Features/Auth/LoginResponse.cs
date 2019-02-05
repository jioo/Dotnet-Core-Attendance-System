using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebApi.Features.Auth
{
    public class LoginResponse
    {
        [JsonProperty("user")]
        public UserDetails User { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }

    public class UserDetails
    {
        [JsonProperty("empId")]
        public Guid EmployeeId { get; set; }
        [JsonProperty("fullName")]
        public string FullName { get; set; }
        [JsonProperty("username")]
        public string UserName { get; set; }
        [JsonProperty("roles")]
        public List<string> Roles { get; set; }
    }
}