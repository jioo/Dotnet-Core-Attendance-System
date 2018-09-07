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
        private readonly IEmployeeService _service;
        
        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }

        // GET api/employee
        /// <summary>
        /// List of employees
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return new OkObjectResult(await _service.GetAllAsync());
        }

        // GET api/employee/id
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Find(Guid id)
        {
            return new OkObjectResult(await _service.FindAsync(id));
        }
        
        // PUT api/employee
        [HttpPut]
        public async Task<IActionResult> Update(EmployeeViewModel model)
        {
            // Check if Card No already exist
            var isCardExist = await _service.isCardExist(model.Id, model.CardNo);
            if(isCardExist) return BadRequest("Card No. is already in use");

            return new OkObjectResult(await _service.UpdateAsync(model));
        }
    }
}