using PAN.Mediator.Abstractions;

namespace Example.Dtos
{
    public class CustomerDto
    {
        public class CreateCustomerCommand : IPanRequest<CreateCustomerResult>
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
        }

        public class CreateCustomerResult
        {
            public CreateCustomerResult(int id, string name, string address, string phone)
            {
                Id = id;
                Name = name;
                Address = address;
                Phone = phone;
            }

            public int Id { get; }
            public string Name { get; }
            public string Address { get; }
            public string Phone { get; }
        }
    }
}
