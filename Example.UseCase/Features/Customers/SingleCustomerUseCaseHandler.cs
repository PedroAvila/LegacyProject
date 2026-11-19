using Example.BuildingBlocks.CQRS;
using Example.BuildingBlocks.Exceptions;
using Example.Ports;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static Example.Dtos.CustomerDto;

namespace Example.UseCase.Features.Customers
{
    public sealed class SingleCustomerUseCaseHandler : IQueryHandler<SingleCustomerQuery, CustomerResponse>
    {
        private readonly ICustomerRepository _customerRepository;

        public SingleCustomerUseCaseHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async ValueTask<CustomerResponse> Handle(SingleCustomerQuery query, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(query.Id);

            if (customer == null)
            {
                throw new BusinessException($"Customer with id {query.Id} was not found.", HttpStatusCode.NotFound);
            }

            return new CustomerResponse(
                customer.Id,
                customer.Name,
                customer.Address,
                customer.Phone);
        }
    }
}
