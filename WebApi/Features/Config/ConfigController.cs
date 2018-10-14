using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;

namespace WebApi.Features.Config
{
    [Authorize]
    [Route("api/[controller]"), ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConfigController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/config
        [HttpGet]
        public async Task<ConfigViewModel> Index()
        {
            return await _mediator.Send(new Details.Query());
        }

        // PUT api/config
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ConfigViewModel> Update(ConfigViewModel viewModel)
        {
            return await _mediator.Send(new Update.Command(viewModel));
        }
    }
}