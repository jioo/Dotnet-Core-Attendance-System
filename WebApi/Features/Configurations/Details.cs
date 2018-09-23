using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure;

namespace WebApi.Features.Configurations
{
    public class Details
    {
        public class Query : IRequest<ConfigurationViewModel> { }

        public class QueryHandler : IRequestHandler<Query, ConfigurationViewModel>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<ConfigurationViewModel> Handle(Query request, CancellationToken cancellationToken)
            {

                try
                {
                    var model = await _context.Configurations.FirstOrDefaultAsync(cancellationToken);
                    return _mapper.Map<ConfigurationViewModel>(model);
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
