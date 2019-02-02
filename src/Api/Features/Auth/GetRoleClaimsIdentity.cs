using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApi.Entities;

namespace WebApi.Features.Auth
{
    public class GetRoleClaimsIdentity
    {
        public class Command : IRequest<ClaimsIdentity>
        {
            public Command(LoginViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public LoginViewModel ViewModel { get; }
        }

        public class CommandHandler : IRequestHandler<Command, ClaimsIdentity>
        {
            private readonly UserManager<User> _manager;

            public CommandHandler(UserManager<User> manager)
            {
                _manager = manager;
            }

            public async Task<ClaimsIdentity> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    // Get the account details
                    var user = await _manager.FindByNameAsync(request.ViewModel.UserName);

                    // Get user roles
                    var roles = await _manager.GetRolesAsync(user);

                    // Generate role claims
                    var roleClaims = new List<Claim>();
                    foreach(var role in roles) 
                    {
                        roleClaims.Add(new Claim(ClaimTypes.Role, role));
                    }
                    
                    return await Task.FromResult(
                        new ClaimsIdentity(roleClaims) 
                    );
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}