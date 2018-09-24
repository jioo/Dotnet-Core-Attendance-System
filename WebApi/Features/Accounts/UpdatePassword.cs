using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApi.Entities;

namespace WebApi.Features.Accounts
{
    public class UpdatePassword
    {
        public class Command : IRequest<IdentityResult>
        {
            public Command(UpdatePasswordViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public UpdatePasswordViewModel ViewModel { get; }
        }

        public class CommandHandler : IRequestHandler<Command, IdentityResult>
        {
            private readonly UserManager<User> _manager;

            public CommandHandler(UserManager<User> manager)
            {
                _manager = manager;
            }

            public async Task<IdentityResult> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    // Get account details
                    var user = await _manager.FindByNameAsync(request.ViewModel.UserName);

                    // Change account password then return result
                    return await _manager.ChangePasswordAsync(
                        user, 
                        request.ViewModel.OldPassword, 
                        request.ViewModel.NewPassword);
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