using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure;

namespace WebApi.Features.Config
{
    public class Update
    {
        public class Command : IRequest<ConfigViewModel>
        {
            public Command(ConfigViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public ConfigViewModel ViewModel { get; }
        }

        public class CommandHandler : IRequestHandler<Command, ConfigViewModel>
        {

            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;

            public CommandHandler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<ConfigViewModel> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    // Get the old model
                    var model = await _context.Config.FirstAsync();

                    // Map the view model into old model
                    _mapper.Map(request.ViewModel, model);

                    // Change entity state to update
                    _context.Entry(model).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    // Map the result to view model
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