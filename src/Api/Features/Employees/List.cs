using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebApi.Infrastructure;
using WebApi.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using AutoMapper;

namespace WebApi.Features.Employees
{
    public class List
    {
        public class Query : IRequest<IList<EmployeeViewModel>> { }

        public class QueryHandler : IRequestHandler<Query, IList<EmployeeViewModel>>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IList<EmployeeViewModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    // Get all employee list
                    var modelList = await _context.Employees
                        .OrderByDescending(m => m.Created)
                        .Include(m => m.Identity)
                        .ToListAsync(cancellationToken);
                    
                    // Map model to view model
                    return _mapper.Map<IList<EmployeeViewModel>>(modelList);
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