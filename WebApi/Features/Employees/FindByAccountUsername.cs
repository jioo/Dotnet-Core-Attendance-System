using System;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using WebApi.Infrastructure;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace WebApi.Features.Employees
{
    public class FindByAccountUsername
    {
        public class Query : IRequest<EmployeeViewModel>
        {
            public Query(string username)
            {
                Username = username;
            }

            public string Username { get; }
        }

        public class QueryHandler : IRequestHandler<Query, EmployeeViewModel>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<EmployeeViewModel> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var result = await _context.Employees
                        .Include(m => m.Identity)
                        .Where(m => m.Identity.UserName == request.Username)
                        .FirstOrDefaultAsync(cancellationToken);

                    return (result != null) 
                        ?  _mapper.Map<EmployeeViewModel>(result)
                        :  new EmployeeViewModel{ FullName = "Administrator" };
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