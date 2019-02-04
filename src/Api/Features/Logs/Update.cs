using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure;

namespace WebApi.Features.Logs
{
    /// <summary>
    /// Update existing log
    /// </summary>
    /// <param name="viewModel">view model</param>
    /// <returns>
    /// Returns <see cref="LogViewModel"/>
    /// </returns>
    public class Update
    {
        public class Command : IRequest<LogViewModel>
        {
            public Command(LogEditViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public LogEditViewModel ViewModel { get; }
        }

        public class CommandHandler : IRequestHandler<Command, LogViewModel>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;

            public CommandHandler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<LogViewModel> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var model = await _context.Logs.FirstOrDefaultAsync(m => m.Id == request.ViewModel.Id);

                    // Validate model
                    if (model == null)
                        return null;

                    // Convert datetime to UTC before updating
                    request.ViewModel.TimeIn = LogExtensions.ToUtc(request.ViewModel.TimeIn.ToString());
                    request.ViewModel.TimeOut =  (request.ViewModel.TimeOut != null) 
                        ? LogExtensions.ToUtc(request.ViewModel.TimeOut.ToString())
                        : request.ViewModel.TimeOut;
                    
                    // Update specifc fields only
                    model.TimeIn = request.ViewModel.TimeIn;
                    model.TimeOut = request.ViewModel.TimeOut;
                    model.Updated = DateTime.UtcNow;

                    _context.Entry(model).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    // Return result with log information
                    return await _context.Logs
                        .Where(m => m.Id == model.Id)
                        .MapToViewModel()
                        .FirstAsync();
                        
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