using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using WebApi.Utils;

namespace WebApi.Features.Config
{
    [Authorize]
    [Route("api/[controller]"), ApiController]
    public class ConfigController : Controller
    {
        private readonly IMediator _mediator;

        public ConfigController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/config
        /// <summary>
        /// Get current atendance config
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ConfigViewModel), StatusCodes.Status200OK)]
        public async Task<ConfigViewModel> Index()
        {
            return await _mediator.Send(new Details.Query());
        }

        // PUT api/config
        /// <summary>
        /// Update attendance config
        /// </summary>
        /// <param name="viewModel"></param>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorHandler), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ConfigViewModel>> Update(ConfigViewModel viewModel)
        {
            string message;
            if (Extensions.ValidateTimeStrings(viewModel, out message))
                return BadRequest(new ErrorHandler{ Description = message });

            return await _mediator.Send(new Update.Command(viewModel));
        }
    }
}