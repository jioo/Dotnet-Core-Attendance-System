using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hubs.BroadcastHub;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApi.Entities;

namespace WebApi.Features.Logs
{
    [Authorize]
    [Route("/api/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<BroadcastHub> _hubContext;

        public LogController(IMediator mediator, IHubContext<BroadcastHub> hubContext)
        {
            _mediator = mediator;
            _hubContext = hubContext;
        }
        
        // GET: api/log
        [HttpGet]
        public async Task<IActionResult> Index(BasePagedList parameters)
        {
            return new OkObjectResult(
                await _mediator.Send(new List.Query(parameters))
            );
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Log(LogInOutViewModel viewModel)
        {
            // Validate card no. & password
            var user = await _mediator.Send(new ValidateTimeInOut.Query(viewModel));
            if (user.Id == Guid.Empty) return BadRequest("Invalid username or password!");

            // Broadcast to web client
            await _hubContext.Clients.All.SendAsync("employee-logged"); 

            // Return result with employee information
            return new OkObjectResult(
                await _mediator.Send(new RecordLog.Command(user))
            );
        }

        // PUT api/log
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<LogViewModel> Update(LogEditViewModel model)
        {
            return await _mediator.Send(new Update.Command(model));
        }
    }
}