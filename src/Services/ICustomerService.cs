using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerApi.Domain;

namespace CustomerApi.Services
{
    public interface ICustomerService
    {
        Task<ServiceResponse<Customer>> CreateAsync(Customer customer);
        Task<ServiceResponse<Customer?>> UpdateAsync(Customer customer);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
        Task<ServiceResponse<Customer?>> GetAsync(int id);
        Task<ServiceResponse<List<Customer>>> GetAllAsync();
    }
}
