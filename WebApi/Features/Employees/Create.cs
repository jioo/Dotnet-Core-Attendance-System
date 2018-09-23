using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using WebApi.Entities;
using WebApi.Infrastructure;

namespace WebApi.Features.Employees
{
    public class Create
    {
        public class Command : IRequest<EmployeeViewModel>
        {
            public Command(EmployeeViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public EmployeeViewModel ViewModel { get; }
        }

        public class CommandHandler : IRequestHandler<Command, EmployeeViewModel>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;

            public CommandHandler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<EmployeeViewModel> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var model = _mapper.Map<Employee>(request.ViewModel);
                    await _context.Employees.AddAsync(model, cancellationToken);
                    await _context.SaveChangesAsync();
                    
                    return _mapper.Map<EmployeeViewModel>(model);
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