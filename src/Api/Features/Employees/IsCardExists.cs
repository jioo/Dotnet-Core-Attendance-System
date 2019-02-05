using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure;

namespace WebApi.Features.Employees
{
    public class IsCardExists
    {
        public class Query : IRequest<bool>
        {
            // Parameters to pass in query
            public Query(Guid id, string cardNo)
            {
                Id = id;
                CardNo = cardNo;
            }

            // Create a properties from parameters
            public Guid Id { get; }
            public string CardNo { get; }
        }

        public class QueryHandler : IRequestHandler<Query, bool>
        {
            private readonly ApplicationDbContext _context;

            public QueryHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var res = await _context.Employees
                        .Where(m => m.Id != request.Id)
                        .Where(m => m.CardNo == request.CardNo)
                        .Where(m => m.Deleted == null)
                        .CountAsync();

                    return (res > 0) ? true: false;
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