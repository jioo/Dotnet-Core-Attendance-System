using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Features.Employees;
using WebApi.Infrastructure;

namespace WebApi.Features.Logs
{
    /// <summary>
    /// Log the time of employee time In/Out
    /// </summary>
    /// <param name="employee">employee model</param>
    /// <returns>
    /// Returns <see cref="LogResultViewModel"/>
    /// </returns>
    public class RecordLog
    {
        public class Command : IRequest<LogResultViewModel>
        {
            public Command(EmployeeViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public EmployeeViewModel ViewModel { get; }
        }

        public class CommandHandler : IRequestHandler<Command, LogResultViewModel>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;

            public CommandHandler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<LogResultViewModel> Handle(Command request, CancellationToken cancellationToken)
            {

                try
                {
                    var newLog = new Log();
                    var log = await _context.Logs
                        .Where(m => m.EmployeeId == request.ViewModel.Id)
                        .Where(m => m.TimeOut == null)
                        .Where(m => m.Deleted == null)
                        .SingleOrDefaultAsync(cancellationToken);

                    // Log in user
                    if (log == null)
                    {
                        newLog.EmployeeId = request.ViewModel.Id;
                        newLog.TimeIn = DateTime.UtcNow;
                        await _context.Logs.AddAsync(newLog);
                    }
                    // Log out user
                    else
                    {
                        var oldModel = await _context.Logs.FindAsync(log.Id);
                        _mapper.Map(log, oldModel);
                        oldModel.TimeOut = DateTime.UtcNow;

                        _context.Entry(oldModel).State = EntityState.Modified;
                    }

                    // Save Insert/Update query
                    await _context.SaveChangesAsync();

                    // Return result with employee information
                    var result = (log != null) ? log : newLog;
                    return new LogResultViewModel
                    {   
                        FullName = result.Employee.FullName,
                        CardNo = request.ViewModel.CardNo,
                        Position = request.ViewModel.Position,
                        TimeIn = LogExtensions.ToLocal(result.TimeIn),
                        TimeOut = (result.TimeOut != null) 
                            ? LogExtensions.ToLocal(result.TimeOut)
                            : string.Empty
                    };
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