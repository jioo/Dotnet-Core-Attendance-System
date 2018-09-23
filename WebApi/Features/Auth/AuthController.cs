using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;

namespace WebApi.Features.Auth
{
    [Route("api/[controller]"), ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST api/auth/login
        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            // Check if credentials are correct
            var validate = await _mediator.Send(new ValidatePassword.Query(viewModel.UserName, viewModel.Password));
            if (!validate) return BadRequest("Invalid username or password");

            // Get User Claims
            var claimsIdentity = await _mediator.Send(new GetRoleClaimsIdentity.Command(viewModel));

            // Get employee information
            var employee = await _mediator.Send(new Employees.FindByAccountUsername.Query(viewModel.UserName));

            // Generate access token for authorization
            return new OkObjectResult(
                await _mediator.Send(new GenerateAccessToken.Command(claimsIdentity, employee, viewModel.UserName))
            );
        }

        // POST api/auth/check
        [Authorize]
        [HttpGet("check")]
        public IActionResult Check()
        {
            return Ok();
        }

        // POST api/auth/is-admin
        [Authorize(Roles = "Admin")]
        [HttpGet("is-admin")]
        public IActionResult IsAdmin()
        {
            return Ok();
        }

        // POST api/auth/is-employee
        [Authorize(Roles = "Employee")]
        [HttpGet("is-employee")]
        public IActionResult IsEmployee()
        {
            return Ok();
        }

        // GET api/auth/xsrfToken
        [HttpGet("xsrfToken")]
        public IActionResult XsrfToken()
        {
            return new OkObjectResult(
                _mediator.Send(new GenerateXsrfToken.Query(HttpContext))
            );
        }
    }
}