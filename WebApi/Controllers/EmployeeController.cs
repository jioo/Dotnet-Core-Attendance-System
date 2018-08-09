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
    public class EmployeeController : ControllerBase
    {
        private JsonSerializerSettings settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        private readonly IEmployeeService _service;
        
        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }

        // GET api/employee
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var res = await _service.GetAllAsync();
            return new OkObjectResult( JsonConvert.SerializeObject(res, settings) );
        }

        // GET api/employee/id
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Find(Guid id)
        {
            var res = await _service.FindAsync(id);
            return new OkObjectResult( JsonConvert.SerializeObject(res, settings) );
        }
        
        
        // PUT api/employee
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]EmployeeViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid Request!");
            }

            if(await _service.isCardExist(model.Id, model.CardNo))
            {
                return BadRequest("Card No. is already in use");
            }

            var res = await _service.UpdateAsync(model);
            return new OkObjectResult( JsonConvert.SerializeObject(res, settings) );
        }
    }
}