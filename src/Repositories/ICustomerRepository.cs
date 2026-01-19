using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerApi.Domain;

namespace CustomerApi.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> AddAsync(Customer customer);
        Task<Customer?> UpdateAsync(Customer customer);
        Task<bool> DeleteAsync(int id);
        Task<Customer?> GetAsync(int id);
        Task<List<Customer>> GetAllAsync();
    }
}
