using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApi.Entities;

namespace WebApi.Features.Accounts
{
    public class Register
    {
        public class Command : IRequest<User>
        {
            public Command(RegisterViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public RegisterViewModel ViewModel { get; }
        }

        public class CommandHandler : IRequestHandler<Command, User>
        {
            private readonly UserManager<User> _manager;

            public CommandHandler(UserManager<User> manager)
            {
                _manager = manager;
            }
            public async Task<User> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    // Create Employee information
                    var employeeInfo = new Employee
                    {
                        FullName = request.ViewModel.FullName,
                        CardNo = request.ViewModel.CardNo,
                        Position = request.ViewModel.Position,
                        Status = Status.Active
                    };

                    var user = new User { UserName = request.ViewModel.UserName, Employee = employeeInfo };
                    var result = await _manager.CreateAsync(user, request.ViewModel.Password);
                    await _manager.AddToRoleAsync(user, "Employee");

                    return user;
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