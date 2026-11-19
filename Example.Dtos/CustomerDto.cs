using Example.BuildingBlocks.CQRS;
using System.Collections.Generic;

namespace Example.Dtos
{
    public class CustomerDto
    {
        public class CreateCustomerCommand : ICommand<CreateCustomerResult>
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

        public class GetCustomerQuery : IQuery<IEnumerable<CustomerResponse>>
        {
            public GetCustomerQuery() { }
        }

        public class SingleCustomerQuery : IQuery<CustomerResponse>
        {
            public SingleCustomerQuery(int id)
            {
                Id = id;
            }

            public int Id { get; }
        }

        public class UpdateCustomerCommand : ICommand<UpdateCustomerResult>
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
        }

        public class UpdateCustomerResult
        {
            public UpdateCustomerResult(int id, string name, string address, string phone)
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

        public class CustomerResponse
        {
            public CustomerResponse(int id, string name, string address, string phone)
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
