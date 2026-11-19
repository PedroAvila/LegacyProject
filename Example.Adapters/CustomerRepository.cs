using Dapper.Contrib.Extensions;
using Example.Entities;
using Example.Ports;
using PAN.DapperLambdaToSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Example.Adapters
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;

        public CustomerRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public async Task<int> CreateAsync(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Dapper.Contrib toma la entidad, resuelve la tabla (Customer -> Customers),
                // excluye la clave [Id] por ser identity y devuelve el valor generado.
                return await connection.InsertAsync(customer);
            }
        }

        public async Task<bool> ExistAsync(Expression<Func<Customer, bool>> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.ExistAsync(predicate);
            }
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Dapper.Contrib resuelve la tabla (Customer -> Customers) y materializa todas las filas.
                return await connection.GetAllAsync<Customer>();
            }
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Dapper.Contrib resuelve la tabla (Customer -> Customers) y la clave [Id].
                return await connection.GetAsync<Customer>(id);
            }
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Se cualifica la llamada porque Dapper.Contrib tambien expone UpdateAsync<T>,
                // lo que haria ambigua la invocacion como metodo de extension. La version de
                // PAN.DapperLambdaToSql ignora las propiedades nulas (actualizacion parcial).
                return await PAN.DapperLambdaToSql.DapperExtensions.UpdateAsync(connection, customer);
            }
        }
    }
}
