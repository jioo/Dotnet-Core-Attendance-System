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
    // [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IRepository<Employee> _repo;
        private readonly IEmployeeService _service;
        private readonly UserManager<User> _manager;
        private readonly IMapper _mapper;

        public AccountsController(
            IRepository<Employee> repo,
            UserManager<User> manager,
            IEmployeeService service,
            IMapper mapper)
        {
            _repo = repo;
            _service = service;
            _manager = manager;
            _mapper = mapper;
        }

        // POST: api/accounts/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request!");
            }

            if (await _service.isCardExist(Guid.Empty, model.CardNo))
            {
                return BadRequest("Card No. is already in use");
            }

            // Create user account
            var user = new User { UserName = model.UserName };
            var result = await _manager.CreateAsync(user, model.Password);
            await _manager.AddToRoleAsync(user, "Employee");

            if (!result.Succeeded) return new BadRequestObjectResult("Username \'" + model.UserName + "\' is already taken");

            try
            {
                // Synchronize account to customer
                var emp = new Employee
                {
                    IdentityId = user.Id,
                    Identity = user,
                    FullName = model.FullName,
                    CardNo = model.CardNo,
                    Position = model.Position
                };

                _repo.Context.Insert(emp);
                await _repo.SaveAsync();
                return new OkObjectResult(JsonConvert.SerializeObject(emp, new JsonSerializerSettings { Formatting = Formatting.Indented }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request!");
            }

            var user = await _manager.FindByNameAsync(model.UserName);
            var result = await _manager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest("Incorrect password");
            }

            return Ok();
        }
    }
}