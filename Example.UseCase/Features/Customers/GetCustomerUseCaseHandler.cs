using Example.BuildingBlocks.CQRS;
using Example.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Example.Dtos.CustomerDto;

namespace Example.UseCase.Features.Customers
{
    public sealed class GetCustomerUseCaseHandler : IQueryHandler<GetCustomerQuery, IEnumerable<CustomerResponse>>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerUseCaseHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async ValueTask<IEnumerable<CustomerResponse>> Handle(GetCustomerQuery query, CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.GetAllAsync();

            return customers.Select(customer => new CustomerResponse(
                customer.Id,
                customer.Name,
                customer.Address,
                customer.Phone));
        }
    }
}
