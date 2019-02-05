using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure;

namespace WebApi.Features.Employees
{
    public class Update
    {
        public class Command : IRequest<EmployeeViewModel>
        {
            // Parameter to pass in query
            public Command(EmployeeViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            // Create a property from parameter
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
                    // Find the old model
                    var model = await _context.Employees.FindAsync(request.ViewModel.Id);

                    // Map the new model into old model
                    _mapper.Map(request.ViewModel, model);

                    // Track updated date
                    model.Updated = DateTime.UtcNow;

                    // Update model
                    _context.Entry(model).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    // Return the updated result by mapping the model to viewModel
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