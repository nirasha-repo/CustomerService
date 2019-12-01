using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerService.Services.Interfaces;
using CustomerService.Models;
using CustomerService.DataContext;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomersDBContext _context;

        public CustomerService(CustomersDBContext context)
        {
            _context = context;
        }

        // To check customer exists or not
        public bool CustomerExists(int id)
        {
            return _context.Customers.Any(c => c.Id == id);
        }

        // Returns customers according to pagination details (pageNo is zero based)
        public async Task<IEnumerable<Customer>> FindCustomers(int pageNo, int pageSize)
        {
            IEnumerable<Customer> customers = await _context.Customers.Skip(pageNo * pageSize).Take(pageSize).ToListAsync();

            return customers;
        }

        // Returns the customer for given customer id
        public async Task<Customer> FindCustomer(int customerId)
        {
            Customer customer = await _context.Customers.FindAsync(customerId);

            return customer;
        }

        // Returns first 10 customers matches their First or Last names
        public async Task<IEnumerable<Customer>> FindCustomers(string searchText)
        {
            IEnumerable<Customer> customers = await (from c in _context.Customers
                                                     where c.FirstName.StartsWith(searchText, StringComparison.CurrentCultureIgnoreCase) ||
                                                           c.LastName.StartsWith(searchText, StringComparison.CurrentCultureIgnoreCase)
                                                     select c).Take(10).ToListAsync();

            return customers;
        }

        // Add customer
        public async Task<Customer> AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        // Update customer
        public async Task<Customer> UpdateCustomer(Customer customer)
        {
            Customer existingCustomer = await _context.Customers.FindAsync(customer.Id);

            existingCustomer.FirstName = customer.FirstName;
            existingCustomer.LastName = customer.LastName;
            existingCustomer.DateOfBirth = customer.DateOfBirth;

            await _context.SaveChangesAsync();

            return customer;
        }

        // Delete customer
        public async Task<Customer> DeleteCustomer(Customer customer)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return customer;
        }
    }
}
