using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CustomerService.DataContext;
using CustomerService.Models;

namespace SampleService.Controllers
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
        // ex: https://localhost:5001/api/customers
        [HttpGet]
        public IEnumerable<Customer> GetCustomers()
        {
            return _context.Customers;
        }

        // GET: api/customers/5
        // ex: https://localhost:5001/api/customers/4
        [HttpGet]
        [Route("{id}")]        
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
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // log exception details
                return StatusCode(500);
            }
            
            return Ok(customer);
        }

        // GET: api/customers
        // ex: https://localhost:5001/api/customers/getCustomerBySearchName/nir
        [HttpGet]
        [Route("getCustomerBySearchName/{searchString}")]
        public IActionResult GetCustomerBySearchName(string searchString)
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

                customer = FindCustomer(searchString);

                if (customer == null)
                {
                    return NotFound("Customer Not Found!");
                }
            }
            catch (Exception ex)
            {
                // log exception details
                return StatusCode(500);
            }            

            return Ok(customer);
        }

        // POST: api/customers
        [HttpPost]
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
                // log exception details
                return StatusCode(500);
            }            
        }

        // PUT: api/customers/5
        [HttpPut("{id}")]
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
                // log exception details
                return StatusCode(500);
            }            
        }

        // DELETE: api/customers/5
        [HttpDelete("{id}")]
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
                // log exception details
                return StatusCode(500);
            }            

            return Ok(customer);
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(c => c.Id == id);
        }

        private Customer FindCustomer(string searchString)
        { 
            Customer customer = _context.Customers.FirstOrDefault(c => c.FirstName.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase));

            if(customer != null)
            {
                return customer;
            }

            customer = _context.Customers.FirstOrDefault(c => c.LastName.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase));

            return customer;
        }
    }
}
