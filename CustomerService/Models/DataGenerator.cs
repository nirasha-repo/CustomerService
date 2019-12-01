using CustomerService.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CustomerService.Models
{
    public class DataGenerator
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new CustomersDBContext(
                serviceProvider.GetRequiredService<DbContextOptions<CustomersDBContext>>()))
            {
                // Look for any customers already in database.
                if (context.Customers.Any())
                {
                    return;   // Database has been seeded
                }

                var customers = GetCustomerData();
                context.Customers.AddRange(customers);

                context.SaveChanges();
            }
        }

        public static IEnumerable<Customer> GetCustomerData()
        {
            var customers = new[]
            {
                new Customer
                {
                    Id = 1,
                    FirstName = "Nirasha",
                    LastName = "Gunasekera",
                    DateOfBirth = new DateTime(1980, 05, 29)
                },
                new Customer
                {
                    Id = 2,
                    FirstName = "Michael",
                    LastName = "Jackson",
                    DateOfBirth = new DateTime(1950, 03, 01)
                },
                new Customer
                {
                    Id = 3,
                    FirstName = "Marilyn",
                    LastName = "Monroe",
                    DateOfBirth = new DateTime(1945, 02, 21)
                },
                new Customer
                {
                    Id = 4,
                    FirstName = "Steve",
                    LastName = "Smith",
                    DateOfBirth = new DateTime(1985, 05, 22)
                },
                new Customer
                {
                    Id = 5,
                    FirstName = "David",
                    LastName = "Warner",
                    DateOfBirth = new DateTime(1986, 11, 10)
                },
                new Customer
                {
                    Id = 6,
                    FirstName = "Shane",
                    LastName = "Warne",
                    DateOfBirth = new DateTime(1975, 08, 14)
                },
                new Customer
                {
                    Id = 7,
                    FirstName = "Shania",
                    LastName = "Twain",
                    DateOfBirth = new DateTime(1980, 02, 11)
                },
                new Customer
                {
                    Id = 8,
                    FirstName = "Mark",
                    LastName = "Shanders",
                    DateOfBirth = new DateTime(1998, 05, 25)
                }
            };

            return customers;
        }
    }
}
