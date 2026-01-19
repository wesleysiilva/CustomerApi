using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CustomerApi.Data;
using CustomerApi.Domain;

namespace CustomerApi.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _db;

        public CustomerRepository(AppDbContext db) => _db = db;

        public async Task<Customer> AddAsync(Customer customer)
        {
            customer.CreatedAt = DateTime.UtcNow;
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer?> UpdateAsync(Customer customer)
        {
            var existing = await _db.Customers.FindAsync(customer.Id);
            if (existing == null) return null;
            existing.FirstName = customer.FirstName;
            existing.LastName = customer.LastName;
            existing.Email = customer.Email;
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Customers.FindAsync(id);
            if (existing == null) return false;
            _db.Customers.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Customer?> GetAsync(int id) => await _db.Customers.FindAsync(id);

        public async Task<List<Customer>> GetAllAsync() => await _db.Customers.ToListAsync();
    }
}
