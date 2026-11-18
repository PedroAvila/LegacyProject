using Example.BuildingBlocks.Exceptions;
using Example.Entities;
using Example.Ports;
using FluentValidation;
using PAN.Mediator.Abstractions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static Example.Dtos.CustomerDto;

namespace Example.UseCase.Features.Customers
{
    public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(command => command.Address)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(command => command.Phone)
                .NotEmpty()
                .MaximumLength(20);
        }
    }

    public class CreateCustomerUseCaseHandler : IPanRequestHandler<CreateCustomerCommand, CreateCustomerResult>
    {
        private readonly ICustomerRepository _customerRepository;

        public CreateCustomerUseCaseHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async ValueTask<CreateCustomerResult> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
        {
            var customer = new Customer
            {
                Name = command.Name,
                Address = command.Address,
                Phone = command.Phone,
            };

            var exists = await _customerRepository.ExistAsync(c => c.Name == customer.Name);

            if (exists)
            {
                throw new BusinessException($"Customer with the same name {customer.Name} already exists.", HttpStatusCode.Conflict);
            }

            var id = await _customerRepository.CreateAsync(customer);

            return new CreateCustomerResult(
                id,
                customer.Name,
                customer.Address,
                customer.Phone);
        }
    }
}
