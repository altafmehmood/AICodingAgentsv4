using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Breach.Api.Features.Breaches;
using System;
using System.Collections.Generic;

namespace Breach.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BreachesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BreachesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetBreachByName(string name)
        {
            var query = new GetBreachByName.Query(name);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> GetRiskAnalysis([FromBody] GetRiskAnalysis.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("report")]
        public async Task<IActionResult> GenerateReport([FromBody] GenerateReport.Query query)
        {
            var result = await _mediator.Send(query);
            return File(result, "application/pdf", "breach-report.pdf");
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllBreaches([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var query = new GetAllBreaches.Query(fromDate, toDate);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetBreachesByEmail(string email)
        {
            var query = new GetBreachesByEmail.Query(email);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}