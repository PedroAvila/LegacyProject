using Example.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Example.Ports
{
    public interface ICustomerRepository
    {
        Task<int> CreateAsync(Customer customer);
        Task<bool> ExistAsync(Expression<Func<Customer, bool>> predicate);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Customer customer);
    }
}
