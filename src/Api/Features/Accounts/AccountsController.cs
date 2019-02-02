using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApi.Entities;
using MediatR;

namespace WebApi.Features.Accounts
{
    [Authorize]
    [Route("api/[controller]"), ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/accounts/register
        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            // mediator from Features/Employees
            var isCardExist = await _mediator.Send(new Employees.IsCardExists.Query(Guid.Empty, viewModel.CardNo));
            if (isCardExist) return BadRequest("Card No. is already in use");

            // mediator from Features/Employees
            var isUsernameExist = await _mediator.Send(new Auth.IsUserExists.Query(viewModel.UserName));
            if(isUsernameExist) return BadRequest($"Username {viewModel.UserName} is already taken");

            // Create user account
            var employeeInfo = await _mediator.Send(new Register.Command(viewModel));

            return new CreatedResult("", employeeInfo);
        }

        // PUT: api/accounts/update-password
        [Authorize(Roles = "Admin")]
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword(ChangePasswordViewModel viewModel)
        {
            // Change a specific Employee account's password
            var result = await _mediator.Send(new UpdatePassword.Command(viewModel));
            if(!result) return StatusCode(500);

            return Ok();
        }
        
        // PUT: api/accounts/change-password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            // Check if Old password is correct
            var validatePassword = await _mediator.Send(new Auth.ValidatePassword.Query(viewModel.UserName, viewModel.OldPassword));
            if (!validatePassword) return BadRequest("Incorrect password");
            
            // Change account password
            var result = await _mediator.Send(new ChangePassword.Command(viewModel));
            if(!result.Succeeded) return BadRequest("Unable to change password");
            
            return Ok();
        }
    }
}