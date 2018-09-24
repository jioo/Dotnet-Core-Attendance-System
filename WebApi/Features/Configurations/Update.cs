using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure;

namespace WebApi.Features.Configurations
{
    public class Update
    {
        public class Command : IRequest
        {
            public Command(ConfigurationViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public ConfigurationViewModel ViewModel { get; }
        }

        public class CommandHandler : AsyncRequestHandler<Command>
        {

            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;

            public CommandHandler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            protected override async Task Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var model = await _context.Configurations.FindAsync(request.ViewModel.Id);
                    _mapper.Map(request.ViewModel, model);

                    _context.Entry(model).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
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