using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure;

namespace WebApi.Features.Config
{
    public class Details
    {
        public class Query : IRequest<ConfigViewModel> { }

        public class QueryHandler : IRequestHandler<Query, ConfigViewModel>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<ConfigViewModel> Handle(Query request, CancellationToken cancellationToken)
            {

                try
                {
                    // Get the current config model
                    var model = await _context.Config.FirstOrDefaultAsync(cancellationToken);

                    // Map model to view model
                    return _mapper.Map<ConfigViewModel>(model);
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
