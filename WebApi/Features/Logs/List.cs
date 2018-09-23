using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure;

namespace WebApi.Features.Logs
{
    /// <summary>
    /// List of logs
    /// </summary>
    public class List
    {
        public class Query : IRequest<IList<LogViewModel>> { }

        public class QueryHandler : IRequestHandler<Query, IList<LogViewModel>>
        {
            private readonly ApplicationDbContext _context;

            public QueryHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IList<LogViewModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    return await _context.Logs
                        .Where(m => m.Deleted == null)
                        .OrderByDescending(m => m.Created)
                        .Select(m => new LogViewModel
                        {
                            Id = m.Id,
                            EmployeeId = m.EmployeeId,
                            TimeIn = LogExtensions.ToLocal(m.TimeIn),
                            TimeOut = (m.TimeOut == null) ? "": LogExtensions.ToLocal(m.TimeOut),
                            Created = m.Created, 
                            Updated = m.Updated,
                            Deleted = m.Deleted,
                            FullName = m.Employee.FullName
                        })
                        .ToListAsync(cancellationToken);
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