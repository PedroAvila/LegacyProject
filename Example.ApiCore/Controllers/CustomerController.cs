using Microsoft.AspNetCore.Mvc;
using PAN.Mediator.Abstractions;
using static Example.Dtos.CustomerDto;

namespace Example.ApiCore.Controllers
{
    [ApiController]
    [Route("api/Customer")]
    public class CustomerController : ControllerBase
    {
        private readonly IPanMediator _mediator;

        public CustomerController(IPanMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var query = new GetCustomerQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Single(int id)
        {
            var query = new SingleCustomerQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerCommand dto)
        {
            var result = await _mediator.Send(dto);
            return Created(string.Empty, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerCommand dto)
        {
            if (dto == null)
            {
                return BadRequest("El cuerpo de la peticion es obligatorio.");
            }

            // El identificador manda desde la ruta, igual que en el resto de APIs REST.
            dto.Id = id;

            var result = await _mediator.Send(dto);
            return Ok(result);
        }
    }
}
