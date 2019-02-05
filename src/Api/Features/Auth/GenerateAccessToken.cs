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
        public class Command : IRequest<LoginResponse>
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

        public class CommandHandler : IRequestHandler<Command, LoginResponse>
        {
            private readonly JwtIssuerOptions _jwtOptions;

            public CommandHandler(IOptions<JwtIssuerOptions> jwtOptions)
            {
                _jwtOptions = jwtOptions.Value;
                Extensions.ThrowIfInvalidOptions(_jwtOptions);
            }

            public async Task<LoginResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    return new LoginResponse
                    {
                        User = new UserDetails
                        {
                            EmployeeId = (request.Employee.Id != Guid.Empty) ? request.Employee.Id: Guid.Empty,
                            FullName = request.Employee.FullName,
                            UserName = request.Uername,
                            Roles = request.ClaimsIdentity.Claims.Where(c => c.Type == ClaimTypes.Role)
                                .Select(c => c.Value)
                                .ToList()
                        },
                        AccessToken = await GenerateEncodedToken(request.Uername, request.ClaimsIdentity),
                        ExpiresIn = (int) _jwtOptions.ValidFor.TotalSeconds
                    };
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