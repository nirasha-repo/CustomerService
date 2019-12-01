using System;
using Xunit;
using CustomerService.Models;
using System.Threading.Tasks;
using CustomerService.Controllers;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CustomerService.Services.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace CustomerServiceTests
{
    public class CustomerControllerTests
    {        
        private readonly ICustomerService _customerService;
        private IEnumerable<Customer> _customers;
        private CustomersController _customersController;

        public CustomerControllerTests()
        {
            _customerService = Substitute.For<ICustomerService>();
            _customers = DataGenerator.GetCustomerData();
            _customersController = new CustomersController(_customerService);
        }
        
        [Fact]
        public async Task GetCustomers_ShouldReturnCorrectType()
        {
            //Arrange            
            var apiTask = Task.FromResult(_customers);
            _customerService.FindCustomers(0, 100).Returns(apiTask);
            
            //Act
            var result = await _customersController.GetCustomers(0, 100);

            //Assert
            Assert.IsAssignableFrom<IEnumerable<Customer>>(result);
        }
       
        [Fact]
        public async Task GetCustomers_ShouldReturnAllCustomers()
        {
            //Arrange            
            var apiTask = Task.FromResult(_customers);
            _customerService.FindCustomers(0, 100).Returns(apiTask);            

            //Act
            var result = await _customersController.GetCustomers(0, 100);
            
            //Assert
            Assert.Equal(8, result.Count());
        }

        [Fact]
        public async Task GetCustomer_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            //Arrange
            _customersController.ModelState.AddModelError("Error", "Modal State is invalid");

            //Act
            var result = await _customersController.GetCustomer(5);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        
        [Fact]
        public async Task GetCustomer_ShouldReturnNotFound_WhenIncorrectCustomerIdIsGiven()
        {
            //Arrange            
            var apiTask = Task.FromResult<Customer>(null);
            _customerService.FindCustomer(99).Returns(apiTask);

            //Act
            var result = await _customersController.GetCustomer(99);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
       
        [Fact]
        public async Task GetCustomer_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            //Arrange
            _customersController.GetCustomer(1).Throws(new Exception());

            //Act
            var result = await _customersController.GetCustomer(1);

            //Assert
            Assert.Equal(500, ((StatusCodeResult) result).StatusCode);
        }
        
        [Fact]
        public async Task GetCustomer_ShouldReturnTheMatchingCustomer_WhenCorrectCustomerIdIsGiven()
        {
            //Arrange
            var apiTask = Task.FromResult(new Customer { Id = 3 });
            _customerService.FindCustomer(3).Returns(apiTask);

            //Act
            var result = await _customersController.GetCustomer(3);

            //Assert
            Assert.Equal(3, ((Customer)((ObjectResult) result).Value).Id);
        }

        [Fact]
        public async Task SearchCustomers_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            //Arrange
            _customersController.ModelState.AddModelError("Error", "Modal State is invalid");

            //Act
            var result = await _customersController.SearchCustomers("test");

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
       
        [Fact]
        public async Task SearchCustomers_ShouldReturnBadRequest_WhenEmptySearchStringIsGiven()
        { 
            //Act
            var result = await _customersController.SearchCustomers("");

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        
        [Fact]
        public async Task SearchCustomers_ShouldReturnNotFound_WhenInvalidSearchStringIsGiven()
        {
            //Arrange
            var apiTask = Task.FromResult<IEnumerable<Customer>>(null);
            _customerService.FindCustomers("zzzzzzzz").Returns(apiTask);

            //Act
            var result = await _customersController.SearchCustomers("zzzzzzzz");

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
        
        [Fact]
        public async Task SearchCustomers_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            //Arrange
            _customerService.FindCustomers("test").Throws(new Exception());

            //Act
            var result = await _customersController.SearchCustomers("test");

            //Assert
            Assert.Equal(500, ((StatusCodeResult)result).StatusCode);
        }
        
        [Fact]
        public async Task SearchCustomers_ShouldReturnMatchingCustomers_WhenGivenSearchStringMatchesWithFirstNameOrLastName()
        {
            //Arrange
            var apiTask = Task.FromResult<IEnumerable<Customer>>(_customers.ToList().FindAll(c => c.Id > 5));
            _customerService.FindCustomers("sha").Returns(apiTask);

            //Act
            var result = await _customersController.SearchCustomers("sha");

            //Assert
            var customers = ((IEnumerable<Customer>)((ObjectResult)result).Value).ToList();

            Assert.Equal(3, customers.Count);

            Assert.Equal("Shane", customers[0].FirstName);
            Assert.Equal("Warne", customers[0].LastName);

            Assert.Equal("Shania", customers[1].FirstName);
            Assert.Equal("Twain", customers[1].LastName);

            Assert.Equal("Mark", customers[2].FirstName);
            Assert.Equal("Shanders", customers[2].LastName);
        }
        
        [Fact]
        public async Task PostCustomer_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            //Arrange         
            _customersController.ModelState.AddModelError("Error", "Modal State is invalid");

            //Act
            var result = await _customersController.PostCustomer(new Customer());

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PostCustomer_ShouldReturnBadRequest_WhenAddingCustomerWithExistingCustomerId()
        {
            //Arrange
            _customerService.CustomerExists(3).Returns(true);

            //Act
            var result = await _customersController.PostCustomer(new Customer { Id = 3 });

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        
        [Fact]
        public async Task PostCustomer_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            //Arrange
            var customer = new Customer();
            _customerService.CustomerExists(3).Returns(false);
            _customerService.AddCustomer(customer).Throws(new Exception());

            //Act
            var result = await _customersController.PostCustomer(customer);

            //Assert
            Assert.Equal(500, ((StatusCodeResult)result).StatusCode);
        }
        
        [Fact]
        public async Task PostCustomer_ShouldReturnNewlyAddedCustomer_WhenNewCustomerIsAdded()
        {
            //Arrange
            var customer = new Customer
            {
                Id = 20,
                FirstName = "Kate",
                LastName = "Winclet",
                DateOfBirth = new DateTime(1973, 05, 21)
            };
        
            _customerService.CustomerExists(20).Returns(false);
            _customerService.AddCustomer(customer).Returns(customer);

            //Act
            var result = await _customersController.PostCustomer(customer);

            //Assert
            Assert.Equal("Kate", ((Customer)((ObjectResult)result).Value).FirstName);
            Assert.Equal("Winclet", ((Customer)((ObjectResult)result).Value).LastName);
        }
        
        [Fact]
        public async Task PutCustomer_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            //Arrange             
            _customersController.ModelState.AddModelError("Error", "Modal State is invalid");

            //Act
            var result = await _customersController.PutCustomer(3, new Customer());

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        
        [Fact]
        public async Task PutCustomer_ShouldReturnBadRequest_WhenCustomerIdsAreNotMatching()
        {   
            //Act
            var result = await _customersController.PutCustomer(3, new Customer { Id = 25 });            

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        
        [Fact]
        public async Task PutCustomer_ShouldReturnNotFound_WhenGivenCustomerIsNotFound()
        {
            //Arrange
            _customerService.CustomerExists(99).Returns(false);

            //Act
            var result = await _customersController.PutCustomer(99, new Customer { Id = 99 });

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
        
        [Fact]
        public async Task PutCustomer_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            //Arrange
            _customerService.CustomerExists(3).Returns(true);
            var customer = new Customer { Id = 3 };
            _customerService.UpdateCustomer(customer).Throws(new Exception());

            //Act
            var result = await _customersController.PutCustomer(3, customer);

            //Assert
            Assert.Equal(500, ((StatusCodeResult)result).StatusCode);
        }
        
        [Fact]
        public async Task PutCustomer_ShouldReturnUpdatedCustomer_WhenExistingCustomerDetailsAreModified()
        {
            //Arrange
            var customer = new Customer
            {
                Id = 3,
                FirstName = "Emma",
                LastName = "Watson",
                DateOfBirth = new DateTime(1993, 05, 21)
            };

            _customerService.CustomerExists(3).Returns(true);
            _customerService.UpdateCustomer(customer).Returns(customer);

            //Act
            var result = await _customersController.PutCustomer(3, customer);

            //Assert
            Assert.Equal("Emma", ((Customer)((ObjectResult)result).Value).FirstName);
            Assert.Equal("Watson", ((Customer)((ObjectResult)result).Value).LastName);
        }
        
        [Fact]
        public async Task DeleteCustomer_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            //Arrange            
            _customersController.ModelState.AddModelError("Error", "Modal State is invalid");

            //Act
            var result = await _customersController.DeleteCustomer(3);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCustomer_ShouldReturnNotFound_WhenGivenCustomerIsNotFound()
        {
            //Arrange
            var apiTask = Task.FromResult<Customer>(null);
            _customerService.FindCustomer(99).Returns(apiTask);

            //Act
            var result = await _customersController.DeleteCustomer(99);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCustomer_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            //Arrange       
            var customer = new Customer();
            _customerService.FindCustomer(4).Returns(customer);
            _customerService.DeleteCustomer(customer).Throws(new Exception());

            //Act
            var result = await _customersController.DeleteCustomer(4);

            //Assert
            Assert.Equal(500, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async Task DeleteCustomer_ShouldReturnDeletedCustomerDetails_WhenCustomerIsDeleted()
        {
            //Arrange
            var customer = new Customer { Id = 4 };
            _customerService.FindCustomer(4).Returns(customer);
            _customerService.DeleteCustomer(customer).Returns(customer);

            //Act
            var result = await _customersController.DeleteCustomer(4);            

            //Assert
            Assert.Equal(4, ((Customer)((ObjectResult)result).Value).Id);                       
        }           
    }
}
