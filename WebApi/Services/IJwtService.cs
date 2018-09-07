using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public interface IJwtService
    {
        /// <summary>
        /// Creates JWT security access token that will be used 
        /// as a credential for accessing authorized endpoints
        /// </summary>
        /// <param name="userName">used for getting account information</param>
        /// <param name="identity">claims identity for jwt</param>
        /// <returns>
        /// Returns <see cref="string"/>
        /// </returns>
        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity);

        /// <summary>
        /// Generate Role Claims based on parameters
        /// </summary>
        /// <param name="roles">roles to generate</param>
        /// <returns>
        /// Returns <see cref="ClaimsIdentity"/>
        /// </returns>
        ClaimsIdentity GenerateRoleClaimsIdentity(IList<string> roles);
    }
}
