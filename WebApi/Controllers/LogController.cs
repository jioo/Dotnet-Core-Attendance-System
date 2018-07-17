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
            var res = await _service.GetAllAsync();
            return new OkObjectResult(JsonConvert.SerializeObject(res, settings));
        }

        // POST api/log
        [HttpPost]
        public async Task<IActionResult> Log([FromBody] LogInOutViewModel model)
        {
            var user = await _service.CheckCardNo(model);
            if (user.Id == Guid.Empty)
            {
                return BadRequest("Invalid username or password!");
            }

            var res = await _service.Log(user);
            await _hubContext.Clients.All.SendAsync("employee-logged"); // broadcast to web client
            return new OkObjectResult(JsonConvert.SerializeObject(res, settings));
        }

        // PUT api/log
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]LogEditViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request!");
                }

                var res = await _service.UpdateAsync(model);
                return new OkObjectResult(JsonConvert.SerializeObject(res, settings));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}