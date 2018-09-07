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

namespace WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigService _service;
        public ConfigController(IConfigService service)
        {
            _service = service;
        }

        // GET api/config
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return new OkObjectResult(await _service.FirstOrDefaultAsync());
        }

        // PUT api/config
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]ConfigurationViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            await _service.UpdateAsync(model);
            return new OkObjectResult("Updated!");
        }
    }
}