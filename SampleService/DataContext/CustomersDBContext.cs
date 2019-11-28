using Microsoft.EntityFrameworkCore;

namespace CustomerService.DataContext
{
    public class CustomersDBContext : DbContext
    {
        public CustomersDBContext(DbContextOptions<CustomersDBContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Customer> Customers { get; set; }
    }
}
