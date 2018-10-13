using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using WebApi.Infrastructure;

namespace WebApi.Features.Logs
{
    public class Details
    {
        public class Query : IRequest<LogViewModel>
        {
            public Query(Guid id)
            {
                Id = id;
            }

            public Guid Id { get; }
        }

        public class QueryHandler : RequestHandler<Query, LogViewModel>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            protected override LogViewModel Handle(Query request)
            {
                try
                {
                    // Find and map model based on id
                    var model = _context.Logs.MapToViewModel()
                        .First(m => m.Id == request.Id);

                    return model;
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