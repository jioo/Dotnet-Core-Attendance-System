using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.Services;
using Newtonsoft.Json;
using WebApi.Entities;
using System.Collections.Generic;

namespace WebApi.Helpers
{
    public class Tokens
    {
        public static async Task<string> GenerateJwt(
            ClaimsIdentity identity,
            IJwtService jwtService,
            string userName,
            JwtIssuerOptions jwtOptions
            )
        {

            var response = new
            {
                user = new
                {
                    username = userName,
                    roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                                .Select(c => c.Value)
                                .ToList()
                },
                access_token = await jwtService.GenerateEncodedToken(userName, identity),
                expires_in = (int)jwtOptions.ValidFor.TotalSeconds
            };

            return JsonConvert.SerializeObject(response);
        }
    }
}
