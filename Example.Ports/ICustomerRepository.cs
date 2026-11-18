using Example.Entities;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Example.Ports
{
    public interface ICustomerRepository
    {
        Task<int> CreateAsync(Customer customer);
        Task<bool> ExistAsync(Expression<Func<Customer, bool>> predicate);
    }
}
