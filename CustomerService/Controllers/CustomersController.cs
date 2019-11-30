using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CustomerService.DataContext;
using CustomerService.Models;
using System.Net;
using Swashbuckle.AspNetCore.SwaggerGen;
using Serilog;

namespace CustomerService.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomersDBContext _context;

        public CustomersController(CustomersDBContext context)
        {
            _context = context;
        }

        // GET: api/customers        
        [HttpGet]
        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // GET: api/customers/5        
        [HttpGet]
        [Route("{id}")]        
        [SwaggerResponse((int)HttpStatusCode.OK, Type = (typeof(Customer)))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCustomer([FromRoute] int id)
        {
            Customer customer;

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                customer = await _context.Customers.FindAsync(id);

                if (customer == null)
                {
                    return NotFound("Customer Not Found!");
                }
            }
            catch (Exception ex)
            {                
                Log.Logger.Error("Get Customer Error : {@CustomerId} {@Error}", id, ex);
                return StatusCode(500);
            }
            
            return Ok(customer);
        }

        // GET: api/customers
        // ex: https://localhost:5001/api/customers/getCustomerBySearchName/nir
        [HttpGet]
        [Route("getCustomerBySearchName/{searchString}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = (typeof(Customer)))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCustomerBySearchName(string searchString)
        {
            Customer customer;

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(searchString))
                {
                    return BadRequest("Search String Is Empty!");
                }

                customer = await FindCustomer(searchString);

                if (customer == null)
                {
                    return NotFound("Customer Not Found!");
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Get Customer By Search Name Error : {@SearchString} {@Error}", searchString, ex);
                return StatusCode(500);
            }            

            return Ok(customer);
        }

        // POST: api/customers
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = (typeof(Customer)))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]        
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostCustomer([FromBody] Customer customer)
        {          
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (CustomerExists(customer.Id))
                {
                    return BadRequest("Customer Already Exists!");
                }

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Post Customer Error : {@CustomerId} {@Error}", customer.Id, ex);
                return StatusCode(500);
            }            
        }

        // PUT: api/customers/5
        [HttpPut("{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = (typeof(Customer)))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PutCustomer([FromRoute] int id, [FromBody] Customer customer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != customer.Id)
                {
                    return BadRequest("Incorrect Customer Ids!");
                }

                if (!CustomerExists(id))
                {
                    return NotFound("Customer Not Found!");
                }
                                
                _context.Entry(customer).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Put Customer Error : {@CustomerId} {@Error}", customer.Id, ex);
                return StatusCode(500);
            }            
        }

        // DELETE: api/customers/5
        [HttpDelete("{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = (typeof(Customer)))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteCustomer([FromRoute] int id)
        {
            Customer customer;

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                customer = await _context.Customers.FindAsync(id);
                
                if (customer == null)
                {
                    return NotFound("Customer Not Found!");
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Delete Customer Error : {@CustomerId} {@Error}", id, ex);
                return StatusCode(500);
            }            

            return Ok(customer);
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(c => c.Id == id);
        }

        private async Task<Customer> FindCustomer(string searchString)
        { 
            Customer customer = await _context.Customers.FirstOrDefaultAsync(c => c.FirstName.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase));

            if(customer != null)
            {
                return customer;
            }

            customer = await _context.Customers.FirstOrDefaultAsync(c => c.LastName.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase));

            return customer;
        }
    }
}
