using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CustomerApi.Domain;
using CustomerApi.Repositories;

namespace CustomerApi.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepository repo, ILogger<CustomerService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<ServiceResponse<Customer>> CreateAsync(Customer customer)
        {
            try
            {
                var created = await _repo.AddAsync(customer);
                return ServiceResponse<Customer>.Success(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return ServiceResponse<Customer>.Fail("An error occurred while creating the customer.");
            }
        }

        public async Task<ServiceResponse<Customer?>> UpdateAsync(Customer customer)
        {
            try
            {
                var updated = await _repo.UpdateAsync(customer);
                if (updated == null) return ServiceResponse<Customer?>.Fail("Customer not found.");
                return ServiceResponse<Customer?>.Success(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer");
                return ServiceResponse<Customer?>.Fail("An error occurred while updating the customer.");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var deleted = await _repo.DeleteAsync(id);
                if (!deleted) return ServiceResponse<bool>.Fail("Customer not found.");
                return ServiceResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer");
                return ServiceResponse<bool>.Fail("An error occurred while deleting the customer.");
            }
        }

        public async Task<ServiceResponse<Customer?>> GetAsync(int id)
        {
            try
            {
                var customer = await _repo.GetAsync(id);
                if (customer == null) return ServiceResponse<Customer?>.Fail("Customer not found.");
                return ServiceResponse<Customer?>.Success(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer");
                return ServiceResponse<Customer?>.Fail("An error occurred while fetching the customer.");
            }
        }

        public async Task<ServiceResponse<List<Customer>>> GetAllAsync()
        {
            try
            {
                var list = await _repo.GetAllAsync();
                return ServiceResponse<List<Customer>>.Success(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customers");
                return ServiceResponse<List<Customer>>.Fail("An error occurred while fetching customers.");
            }
        }
    }
}
