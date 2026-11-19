using PAN.Mediator.Abstractions;
using System.Threading.Tasks;
using System.Web.Http;
using static Example.Dtos.CustomerDto;

namespace Example.Api.Controllers
{
    [RoutePrefix("api/Customer")]
    public class CustomerController : ApiController
    {
        private readonly IPanMediator _mediator;

        public CustomerController(IPanMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var query = new GetCustomerQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Single(int id)
        {
            var query = new SingleCustomerQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Create([FromBody] CreateCustomerCommand dto)
        {
            var result = await _mediator.Send(dto);
            return Created(string.Empty, result);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] UpdateCustomerCommand dto)
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
