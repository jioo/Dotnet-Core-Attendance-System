using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Newtonsoft.Json;
using WebApi.Entities;
using WebApi.ViewModels;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmployeeService _service;
        private readonly IJwtService _jwtService;
        private readonly JwtIssuerOptions _jwtOptions;

        public AuthController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmployeeService service,
            IJwtService jwtService,
            IOptions<JwtIssuerOptions> jwtOptions)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _service = service;
            _jwtService = jwtService;
            _jwtOptions = jwtOptions.Value;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            // Check if password is correct
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (!await _userManager.CheckPasswordAsync(user, model.Password)) 
                return BadRequest("Invalid username or password"); 

            // Get User Claims
            var identity = await GetClaimsIdentity(model.UserName, model.Password);

            // Check if account does not exist
            if (identity == null) return BadRequest("Invalid username or password");

            // Get employee information
            var employee = await _service.GetEmployeeByUserId(user.Id);

            // Generate access token for authorization
            var jwt = await Tokens.GenerateJwt(identity, _jwtService, employee.Id, employee.FullName, model.UserName, _jwtOptions);
            return new OkObjectResult(jwt);
        }

        // POST api/auth/check
        [HttpGet("check")]
        [Authorize]
        public IActionResult Check()
        {
            return Ok();
        }

        // POST api/auth/is-admin
        [HttpGet("is-admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult IsAdmin()
        {
            return Ok();
        }

        // POST api/auth/is-employee
        [HttpGet("is-employee")]
        [Authorize(Roles = "Employee")]
        public IActionResult IsEmployee()
        {
            return Ok();
        }

        #region Helpers
        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            // Get the user to verifty
            var userToVerify = await _userManager.FindByNameAsync(userName);

            // Get roles
            var roles = await _userManager.GetRolesAsync(userToVerify);

            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

            // Check the credentials
            if (await _userManager.CheckPasswordAsync(userToVerify, password))
            {
                // Generate Claims
                return await Task.FromResult(_jwtService.GenerateRoleClaimsIdentity(roles));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }
        #endregion
    }
}