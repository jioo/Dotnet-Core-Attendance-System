using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Utils;

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
        /// <summary>
        /// List of employee's account and details
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(EmployeeViewModel), StatusCodes.Status200OK)]
        public async Task<IList<EmployeeViewModel>> Index()
        {
            return await _mediator.Send(new List.Query());
        }

        // GET api/employee/{id}
        /// <summary>
        /// Get employee details
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EmployeeViewModel), StatusCodes.Status200OK)]
        public async Task<EmployeeViewModel> Details(Guid id)
        {
            return await _mediator.Send(new Details.Query(id));
        }
        
        // PUT api/employee
        /// <summary>
        /// Update employee details
        /// </summary>
        /// <remarks>
        /// Unique card no. filter will be applied
        /// </remarks>
        /// <param name="model"></param>
        [HttpPut]
        [ProducesResponseType(typeof(EmployeeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorHandler), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(EmployeeViewModel model)
        {
            // Check if Card No already exists
            var isCardExist = await _mediator.Send(
                new IsCardExists.Query(model.Id, model.CardNo)
            );
            
            if (isCardExist) 
                return BadRequest(new ErrorHandler{ Description = "Card No. is already in use." });

            // Return update result
            return new OkObjectResult(
                await _mediator.Send(new Update.Command(model))
            );
        }
    }
}