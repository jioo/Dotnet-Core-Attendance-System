using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebApi.Entities;
using WebApi.Features.Employees;

namespace WebApi.Features.Accounts
{
    public class Register
    {
        public class Command : IRequest<EmployeeViewModel>
        {
            public Command(RegisterViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public RegisterViewModel ViewModel { get; }
        }

        public class CommandHandler : IRequestHandler<Command, EmployeeViewModel>
        {
            private readonly IMapper _mapper;
            private readonly UserManager<User> _manager;

            public CommandHandler(IMapper mapper, UserManager<User> manager)
            {
                _mapper = mapper;
                _manager = manager;
            }
            public async Task<EmployeeViewModel> Handle(Command request, CancellationToken cancellationToken)
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

                    return _mapper.Map<EmployeeViewModel>(user.Employee);
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