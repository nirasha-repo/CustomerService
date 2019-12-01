using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CustomerService.Models;
using System.Net;
using Swashbuckle.AspNetCore.SwaggerGen;
using Serilog;
using CustomerService.Services.Interfaces;

namespace CustomerService.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: api/customers        
        [HttpGet]
        public async Task<IEnumerable<Customer>> GetCustomers([FromQuery]int pageNo, [FromQuery]int pageSize)
        {
            return await _customerService.FindCustomers(pageNo, pageSize);
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

                customer = await _customerService.FindCustomer(id);

                if (customer == null)
                {
                    return NotFound("Customer Not Found!");
                }
            }
            catch (Exception ex)
            {                
                Log.Logger.Error("Get Customer Error for Customer Id: {@CustomerId} {@Error}", id, ex);
                return StatusCode(500);
            }
            
            return Ok(customer);
        }

        
        // GET: api/customers
        // ex: https://localhost:5001/api/customers/searchCustomer/nir
        [HttpGet]
        [Route("searchCustomers/{name}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = (typeof(Customer)))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SearchCustomers(string name)
        {
            IEnumerable<Customer> customers;

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest("Search Text Is Empty!");
                }

                customers = await _customerService.FindCustomers(name);

                if (customers == null || !customers.Any())
                {
                    return NotFound("Customer Not Found!");
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Search Customer Error for the name: {@SearchString} {@Error}", name, ex);
                return StatusCode(500);
            }            

            return Ok(customers);
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

                if (_customerService.CustomerExists(customer.Id))
                {
                    return BadRequest("Customer Already Exists!");
                }

                Customer newCustomer = await _customerService.AddCustomer(customer);

                return CreatedAtAction("GetCustomer", new { id = customer.Id }, newCustomer);
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Post Customer Error for Customer Id: {@CustomerId} {@Error}", customer.Id, ex);
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

                if (!_customerService.CustomerExists(id))
                {
                    return NotFound("Customer Not Found!");
                }

                Customer updatedCustomer = await _customerService.UpdateCustomer(customer);

                return CreatedAtAction("GetCustomer", new { id = customer.Id }, updatedCustomer);
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Put Customer Error for Customer Id: {@CustomerId} {@Error}", customer.Id, ex);
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

                customer = await _customerService.FindCustomer(id);

                if (customer == null)
                {
                    return NotFound("Customer Not Found!");
                }

                customer = await _customerService.DeleteCustomer(customer);
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Delete Customer Error for Customer Id: {@CustomerId} {@Error}", id, ex);
                return StatusCode(500);
            }            

            return Ok(customer);
        }       
    }
}
