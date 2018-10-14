using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace WebApi.Features.Auth
{
    [Route("api/[controller]"), ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContext;

        public AuthController(IMediator mediator, IHttpContextAccessor httpContext)
        {
            _mediator = mediator;
            _httpContext = httpContext;
        }

        // POST api/auth/login
        [HttpPost("login")]
        // [ValidateAntiForgeryToken]
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

        // POST api/challenge/{role}
        [Authorize]
        [HttpPost("challenge/{role?}")]
        public IActionResult ChallengeAuth(string role)
        {
            if(!string.IsNullOrEmpty(role)) 
            {
                var validateRole = _httpContext.HttpContext.User.IsInRole(role);
                if(!validateRole)
                {
                    return Forbid();
                }
            }

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