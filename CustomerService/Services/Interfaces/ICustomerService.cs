using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerService.Models;

namespace CustomerService.Services.Interfaces
{
    public interface ICustomerService
    {
        bool CustomerExists(int id);
        Task<IEnumerable<Customer>> FindCustomers(int pageNo, int pageSize);
        Task<Customer> FindCustomer(int customerId);
        Task<IEnumerable<Customer>> FindCustomers(string searchText);
        Task<Customer> AddCustomer(Customer customer);
        Task<Customer> UpdateCustomer(Customer customer);
        Task<Customer> DeleteCustomer(Customer customer);
    }
}
