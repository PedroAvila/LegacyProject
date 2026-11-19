using Example.BuildingBlocks.CQRS;
using Example.BuildingBlocks.Exceptions;
using Example.Entities;
using Example.Ports;
using FluentValidation;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static Example.Dtos.CustomerDto;

namespace Example.UseCase.Features.Customers
{
    public sealed class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(command => command.Id)
                .GreaterThan(0);

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

    public sealed class UpdateCustomerUseCaseHandler : ICommandHandler<UpdateCustomerCommand, UpdateCustomerResult>
    {
        private readonly ICustomerRepository _customerRepository;

        public UpdateCustomerUseCaseHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async ValueTask<UpdateCustomerResult> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken)
        {
            var exists = await _customerRepository.ExistAsync(c => c.Id == command.Id);

            if (!exists)
            {
                throw new BusinessException($"Customer with id {command.Id} was not found.", HttpStatusCode.NotFound);
            }

            var duplicatedName = await _customerRepository.ExistAsync(
                c => c.Name == command.Name && c.Id != command.Id);

            if (duplicatedName)
            {
                throw new BusinessException($"Customer with the same name {command.Name} already exists.", HttpStatusCode.Conflict);
            }

            var customer = new Customer
            {
                Id = command.Id,
                Name = command.Name,
                Address = command.Address,
                Phone = command.Phone,
            };

            var updated = await _customerRepository.UpdateAsync(customer);

            if (!updated)
            {
                throw new BusinessException($"Customer with id {command.Id} was not found.", HttpStatusCode.NotFound);
            }

            return new UpdateCustomerResult(
                customer.Id,
                customer.Name,
                customer.Address,
                customer.Phone);
        }
    }
}
