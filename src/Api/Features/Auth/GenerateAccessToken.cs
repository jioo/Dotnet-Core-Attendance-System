using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using WebApi.Entities;
using WebApi.Features.Employees;

namespace WebApi.Features.Auth
{
    public class GenerateAccessToken
    {
        public class Command : IRequest<Object>
        {
            public Command(
                ClaimsIdentity claimsIdentity,
                EmployeeViewModel employee,
                string uername)
            {
                ClaimsIdentity = claimsIdentity;
                Employee = employee;
                Uername = uername;
            }

            public ClaimsIdentity ClaimsIdentity { get; }
            public EmployeeViewModel Employee { get; }
            public string Uername { get; }
        }

        public class CommandHandler : IRequestHandler<Command, Object>
        {
            private readonly JwtIssuerOptions _jwtOptions;

            public CommandHandler(IOptions<JwtIssuerOptions> jwtOptions)
            {
                _jwtOptions = jwtOptions.Value;
                Extensions.ThrowIfInvalidOptions(_jwtOptions);
            }

            public async Task<object> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var response = new
                    {
                        user = new
                        {
                            empId = (request.Employee.Id != Guid.Empty) ? request.Employee.Id: Guid.Empty,
                            fullName = request.Employee.FullName,
                            username = request.Uername,
                            roles = request.ClaimsIdentity.Claims.Where(c => c.Type == ClaimTypes.Role)
                                        .Select(c => c.Value)
                                        .ToList()
                        },
                        access_token = await GenerateEncodedToken(request.Uername, request.ClaimsIdentity),
                        expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
                    };

                    return response;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            public async Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userName),
                    new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                    new Claim(JwtRegisteredClaimNames.Iat, Extensions.ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                    identity.FindFirst(ClaimTypes.Role)
                };

                // Create the JWT security token and encode it.
                var jwt = new JwtSecurityToken(
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    claims: claims,
                    notBefore: _jwtOptions.NotBefore,
                    expires: _jwtOptions.Expiration,
                    signingCredentials: _jwtOptions.SigningCredentials);

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                return encodedJwt;
            }
        }
    }
}