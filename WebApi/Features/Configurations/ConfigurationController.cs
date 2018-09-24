using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;

namespace WebApi.Features.Configurations
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]"), ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConfigurationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/configuration
        [HttpGet]
        public async Task<ConfigurationViewModel> Index()
        {
            return await _mediator.Send(new Details.Query());
        }

        // PUT api/configuration
        [HttpPut]
        public async Task<IActionResult> Update(ConfigurationViewModel viewModel)
        {
            await _mediator.Send(new Update.Command(viewModel));
            return new OkResult();
        }
    }
}