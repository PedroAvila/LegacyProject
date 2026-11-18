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

        [HttpPost]
        public async Task<IHttpActionResult> Create([FromBody] CreateCustomerCommand dto)
        {
            var result = await _mediator.Send(dto);
            return Created(string.Empty, result);
        }
    }
}
