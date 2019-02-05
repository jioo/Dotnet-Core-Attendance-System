using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hubs.BroadcastHub;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApi.Entities;
using WebApi.Utils;

namespace WebApi.Features.Logs
{
    [Authorize]
    [Route("/api/[controller]"), ApiController]
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
        /// <summary>
        /// List of employee logs
        /// </summary>
        /// <remarks>
        /// Server side list that includes sort, search, and paging.
        /// <br/>
        /// 
        /// Valid Parameters:
        /// - search: Search `FullName` field.
        /// - page: Get a specific page. Default value = 1
        /// - rowsPerPage: Define number of items per page. Default value = 10
        /// - descending: (boolean) Sort by descending: Default = null
        /// - sortBy:  Sort entries by specific attribute. Valid fields = [`fullName`, `timeIn`, `timeOut`]
        /// - startDate and endDate: Apply date range filter
        /// 
        /// Sample Requests:
        /// <![CDATA[
        /// - /api/log?search=Justine
        /// - /api/log?sortBy=fullName&descending=true
        /// - /api/log?startDate=2019-02-01&endDate=2019-02-05
        /// ]]>
        /// </remarks>
        /// <param name="parameters"></param>
        [HttpGet]
        [ProducesResponseType(typeof(ListResponse<LogViewModel>), StatusCodes.Status200OK)]
        public async Task<ListResponse<LogViewModel>> Index([FromQuery] DateFilteredList parameters)
        {
            return await _mediator.Send(new List.Query(parameters));
        }

        // GET: api/log/{id}
        /// <summary>
        /// Get log details
        /// </summary>
        /// <param name="id"></param>
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(LogViewModel), StatusCodes.Status200OK)]
        public async Task<LogViewModel> Details(Guid id)
        {
            return await _mediator.Send(new Details.Query(id));
        }

        // POST: api/log
        /// <summary>
        /// Record new log
        /// </summary>
        /// <param name="viewModel"></param>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(LogResultViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorHandler), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Log(LogInOutViewModel viewModel)
        {
            // Validate card no. & password
            var user = await _mediator.Send(new ValidateTimeInOut.Query(viewModel));
            if (user == null) 
                return BadRequest(new ErrorHandler{ Description = "Invalid card no. or password." });

            // Broadcast to web client
            await _hubContext.Clients.All.SendAsync("employee-logged"); 

            // Return result with employee information
            return new OkObjectResult(
                await _mediator.Send(new RecordLog.Command(user))
            );
        }

        // PUT api/log
        /// <summary>
        /// Update a specific log time in or time out
        /// </summary>
        /// <param name="model"></param>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [ProducesResponseType(typeof(LogViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorHandler), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LogViewModel>> Update(LogEditViewModel model)
        {
            var result = await _mediator.Send(new Update.Command(model));

            if (result == null)
                return BadRequest(new ErrorHandler{ Description = "Unable to update data." });

            return result;
        }
    }
}