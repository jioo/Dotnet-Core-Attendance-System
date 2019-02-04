using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Features.Employees;
using WebApi.Infrastructure;

namespace WebApi.Features.Logs
{
    /// <summary>
    /// Validate employee Time In/Out
    /// </summary>
    /// <param name="viewModel">view model</param>
    /// <returns>
    /// Returns <see cref="EmployeeViewModel"/>
    /// </returns>
    public class ValidateTimeInOut
    {
        public class Query : IRequest<EmployeeViewModel>
        {
            public Query(LogInOutViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public LogInOutViewModel ViewModel { get; }
        }

        public class QueryHandler : IRequestHandler<Query, EmployeeViewModel>
        {
            private readonly ApplicationDbContext _context;
            private readonly UserManager<User> _manager;
            private readonly IMapper _mapper;

            public QueryHandler(
                ApplicationDbContext context,
                UserManager<User> manager,
                IMapper mapper)
            {
                _context = context;
                _manager = manager;
                _mapper = mapper;
            }

            public async Task<EmployeeViewModel> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    // Get employee information
                    var emp = await _context.Employees
                        .Where(m => m.CardNo == request.ViewModel.CardNo)
                        .Where(m => m.Status == Entities.Status.Active)
                        .Where(m => m.Deleted == null)
                        .SingleOrDefaultAsync(cancellationToken);
                    
                    // Check if employee exist
                    if (emp == null) 
                        return new EmployeeViewModel{ Id = Guid.Empty };

                    // Check if password is correct
                    var user = await _manager.FindByIdAsync(emp.IdentityId);
                    if (!await _manager.CheckPasswordAsync(user, request.ViewModel.Password))
                        return new EmployeeViewModel{ Id = Guid.Empty };

                    return _mapper.Map<EmployeeViewModel>(emp);
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