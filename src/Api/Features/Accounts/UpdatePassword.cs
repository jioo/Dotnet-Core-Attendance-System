
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApi.Entities;

namespace WebApi.Features.Accounts
{
    public class UpdatePassword
    {
        public class Command : IRequest<bool>
        {
            public Command(UpdatePasswordViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public UpdatePasswordViewModel ViewModel { get; }
        }

        public class CommandHandler : IRequestHandler<Command, bool>
        {
            private readonly UserManager<User> _manager;

            public CommandHandler(UserManager<User> manager)
            {
                _manager = manager;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    // Validate User
                    var validateUser = _manager.Users.FirstOrDefault(m => m.UserName == request.ViewModel.UserName);
                    if(validateUser == null)
                        return false;
                        
                    // Get account details
                    var user = await _manager.FindByNameAsync(request.ViewModel.UserName);

                    // Remove the existing password
                    await _manager.RemovePasswordAsync(user);

                    // Add the new password
                    var result = await _manager.AddPasswordAsync(user, request.ViewModel.Password);

                    return (result.Succeeded) ? true : false;
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