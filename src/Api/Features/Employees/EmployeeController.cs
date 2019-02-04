using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Features.Employees
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]"), ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/employee
        [HttpGet]
        public async Task<IList<EmployeeViewModel>> Index()
        {
            return await _mediator.Send(new List.Query());
        }

        // GET api/employee/{id}
        [HttpGet("{id:guid}")]
        public async Task<EmployeeViewModel> Details(Guid id)
        {
            return await _mediator.Send(new Details.Query(id));
        }
        
        // PUT api/employee
        [HttpPut]
        public async Task<IActionResult> Update(EmployeeViewModel model)
        {
            // Check if Card No already exists
            var isCardExist = await _mediator.Send(
                new IsCardExists.Query(model.Id, model.CardNo)
            );
            if (isCardExist) 
                return BadRequest("Card No. is already in use");

            // Return update result
            return new OkObjectResult(
                await _mediator.Send(new Update.Command(model))
            );
        }
    }
}