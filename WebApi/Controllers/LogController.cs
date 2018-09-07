using System;
using System.Data;
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
using WebApi.Services;
using Microsoft.AspNetCore.SignalR;
using Hubs.BroadcastHub;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private JsonSerializerSettings settings = new JsonSerializerSettings { Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        private readonly ILogService _service;
        private readonly IHubContext<BroadcastHub> _hubContext;

        public LogController(ILogService service, IHubContext<BroadcastHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        // GET api/log
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return new OkObjectResult(await _service.GetAllAsync());
        }

        // POST api/log
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Log([FromBody] LogInOutViewModel model)
        {
            // Validate card no. & password
            var user = await _service.ValidateTimeInOutCredentials(model);
            if (user.Id == Guid.Empty) return BadRequest("Invalid username or password!");

            // Broadcast to web client
            await _hubContext.Clients.All.SendAsync("employee-logged"); 
            return new OkObjectResult(await _service.Log(user));
        }

        // PUT api/log
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody]LogEditViewModel model)
        {
            return new OkObjectResult(await _service.UpdateAsync(model));
        }
    }
}