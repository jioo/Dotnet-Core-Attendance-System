using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AutoMapper;
using WebApi.Infrastructures;
using WebApi.Entities;
using WebApi.ViewModels;
using WebApi.Helpers;
using WebApi.Repositories;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]"), ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<User> _manager;
        private readonly IEmployeeService _service;
        private readonly IMapper _mapper;

        public AccountsController(
            UserManager<User> manager,
            IEmployeeService service,
            IMapper mapper)
        {
            _manager = manager;
            _service = service;
            _mapper = mapper;
        }

        // POST: api/accounts/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var isCardExist = await _service.isCardExist(Guid.Empty, model.CardNo);
            if (isCardExist)
            {
                return BadRequest("Card No. is already in use");
            }

            var isUsernameExist = await _manager.FindByNameAsync(model.UserName);
            if(isUsernameExist != null)
            {
                return BadRequest($"Username {model.UserName} is already taken");
            }

            // Create user account
            var user = new User { UserName = model.UserName };
            var result = await _manager.CreateAsync(user, model.Password);
            await _manager.AddToRoleAsync(user, "Employee");

            // Check if account is successfully registered
            if (!result.Succeeded) return new BadRequestObjectResult("Unable to register account");

            // Synchronize new account to employee information
            var syncResult = await _service.AddAsync(new EmployeeViewModel
            {
                IdentityId = user.Id,
                Identity = user,
                FullName = model.FullName,
                CardNo = model.CardNo,
                Position = model.Position,
                Status = Status.Active
            });
            return new OkObjectResult(syncResult);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordViewModel model)
        {
            // Check if Old password is correct
            var user = await _manager.FindByNameAsync(model.UserName);
            if(!await _manager.CheckPasswordAsync(user, model.OldPassword))
            {
                return BadRequest("Incorrect password");
            }
            
            // Change account password
            var result = await _manager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if(!result.Succeeded) return BadRequest("Unable to change password");
            
            return Ok();
        }
    }
}