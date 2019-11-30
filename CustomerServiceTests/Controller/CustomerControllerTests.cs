using System;
using Xunit;
using CustomerService.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CustomerService.Models;
using System.Threading.Tasks;
using CustomerService.Controllers;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace CustomerServiceTests
{
    public class CustomerControllerTests : IDisposable
    {
        private readonly CustomersDBContext _context;

        public CustomerControllerTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<CustomersDBContext>()
                .UseInMemoryDatabase("Customers")
                .UseInternalServiceProvider(serviceProvider)
                .EnableSensitiveDataLogging()
                .Options;            

            _context = new CustomersDBContext(options);
            _context.Database.EnsureCreated();

            Seed(_context);
        }

        [Fact]
        public async Task GetCustomers_ShouldReturnCorrectType()
        {
            //Arrange
            var controller = new CustomersController(_context);

            //Act
            var result = await controller.GetCustomers();

            //Assert
            Assert.IsAssignableFrom<IEnumerable<Customer>>(result);
        }

        [Fact]
        public async Task GetCustomers_ShouldReturnAllCustomers()
        {
            //Arrange
            var controller = new CustomersController(_context);

            //Act
            var result = await controller.GetCustomers();
            
            //Assert
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task GetCustomer_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            //Arrange
            var controller = new CustomersController(_context);
            controller.ModelState.AddModelError("Error", "Modal State is invalid");

            //Act
            var result = await controller.GetCustomer(5);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetCustomer_ShouldReturnNotFound_WhenIncorrectCustomerIdIsGiven()
        {
            //Arrange
            var controller = new CustomersController(_context);            

            //Act
            var result = await controller.GetCustomer(99);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetCustomer_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            //Arrange
            var controller = new CustomersController(null);

            //Act
            var result = await controller.GetCustomer(1);            

            //Assert
            Assert.Equal(500, ((StatusCodeResult) result).StatusCode);
        }

        [Fact]
        public async Task GetCustomer_ShouldReturnTheMatchingCustomer_WhenCorrectCustomerIdIsGiven()
        {
            //Arrange
            var controller = new CustomersController(_context);

            //Act
            var result = await controller.GetCustomer(3);

            //Assert
            Assert.Equal(3, ((Customer)((ObjectResult) result).Value).Id);
        }

        [Fact]
        public async Task GetCustomerBySearchName_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            //Arrange
            var controller = new CustomersController(_context);
            controller.ModelState.AddModelError("Error", "Modal State is invalid");

            //Act
            var result = await controller.GetCustomerBySearchName("test");

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetCustomerBySearchName_ShouldReturnBadRequest_WhenEmptySearchStringIsGiven()
        {
            //Arrange
            var controller = new CustomersController(_context);            

            //Act
            var result = await controller.GetCustomerBySearchName("");

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetCustomerBySearchName_ShouldReturnNotFound_WhenInvalidSearchStringIsGiven()
        {
            //Arrange
            var controller = new CustomersController(_context);

            //Act
            var result = await controller.GetCustomerBySearchName("zzzzzzzz");

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetCustomerBySearchName_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            //Arrange
            var controller = new CustomersController(null);

            //Act
            var result = await controller.GetCustomerBySearchName("test");

            //Assert
            Assert.Equal(500, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async Task GetCustomerBySearchName_ShouldReturnTheMatchingCustomer_WhenGivenSearchStringMatchesWithFirstName()
        {
            //Arrange
            var controller = new CustomersController(_context);

            //Act
            var result = await controller.GetCustomerBySearchName("nira");

            //Assert
            Assert.Equal("Nirasha", ((Customer)((ObjectResult)result).Value).FirstName);
            Assert.Equal("Gunasekera", ((Customer)((ObjectResult)result).Value).LastName);
        }

        [Fact]
        public async Task GetCustomerBySearchName_ShouldReturnTheMatchingCustomer_WhenGivenSearchStringMatchesWithLastName()
        {
            //Arrange
            var controller = new CustomersController(_context);

            //Act
            var result = await controller.GetCustomerBySearchName("guna");

            //Assert
            Assert.Equal("Nirasha", ((Customer)((ObjectResult)result).Value).FirstName);
            Assert.Equal("Gunasekera", ((Customer)((ObjectResult)result).Value).LastName);
        }

        [Fact]
        public async Task PostCustomer_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            //Arrange
            var controller = new CustomersController(_context);
            controller.ModelState.AddModelError("Error", "Modal State is invalid");

            //Act
            var result = await controller.PostCustomer(new Customer());

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PostCustomer_ShouldReturnBadRequest_WhenAddingCustomerWithExistingCustomerId()
        {
            //Arrange
            var controller = new CustomersController(_context);
            controller.ModelState.AddModelError("Error", "Modal State is invalid");

            //Act
            var result = await controller.PostCustomer(new Customer { Id = 3 });

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PostCustomer_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            //Arrange
            var controller = new CustomersController(null);

            //Act
            var result = await controller.PostCustomer(new Customer());

            //Assert
            Assert.Equal(500, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async Task PostCustomer_ShouldReturnNewlyAddedCustomer_WhenNewCustomerIsAdded()
        {
            //Arrange
            var controller = new CustomersController(_context);

            //Act
            var result = await controller.PostCustomer(
                new Customer
                {
                    Id = 20,
                    FirstName = "Kate",
                    LastName = "Winclet",
                    DateOfBirth = new DateTime(1973, 05, 21)
                });

            //Assert
            Assert.Equal("Kate", ((Customer)((ObjectResult)result).Value).FirstName);
            Assert.Equal("Winclet", ((Customer)((ObjectResult)result).Value).LastName);
        }

        [Fact]
        public async Task PutCustomer_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            //Arrange
            var controller = new CustomersController(_context);
            controller.ModelState.AddModelError("Error", "Modal State is invalid");

            //Act
            var result = await controller.PutCustomer(3, new Customer());

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutCustomer_ShouldReturnBadRequest_WhenCustomerIdsAreNotMatching()
        {
            //Arrange
            var controller = new CustomersController(_context);

            //Act
            var result = await controller.PutCustomer(3, new Customer { Id = 25 });            

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutCustomer_ShouldReturnNotFound_WhenGivenCustomerIsNotFound()
        {
            //Arrange
            var controller = new CustomersController(_context);

            //Act
            var result = await controller.PutCustomer(99, new Customer { Id = 99 });

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PutCustomer_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            //Arrange
            var controller = new CustomersController(null);

            //Act
            var result = await controller.PutCustomer(3, new Customer { Id = 3 });

            //Assert
            Assert.Equal(500, ((StatusCodeResult)result).StatusCode);
        }

        //[Fact]
        //public async Task PutCustomer_ShouldReturnUpdatedCustomer_WhenExistingCustomerDetailsAreModified()
        //{
        //    //Arrange
        //    var controller = new CustomersController(_context);

        //    //Act
        //    var result = await controller.PutCustomer(3, 
        //        new Customer
        //        {
        //            Id = 3,
        //            FirstName = "Emma",
        //            LastName = "Watson",
        //            DateOfBirth = new DateTime(1993, 05, 21)
        //        });

        //    //Assert
        //    Assert.Equal("Emma", ((Customer)((ObjectResult)result).Value).FirstName);
        //    Assert.Equal("Watson", ((Customer)((ObjectResult)result).Value).LastName);
        //}

        [Fact]
        public async Task DeleteCustomer_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            //Arrange
            var controller = new CustomersController(_context);
            controller.ModelState.AddModelError("Error", "Modal State is invalid");

            //Act
            var result = await controller.DeleteCustomer(3);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCustomer_ShouldReturnNotFound_WhenGivenCustomerIsNotFound()
        {
            //Arrange
            var controller = new CustomersController(_context);

            //Act
            var result = await controller.DeleteCustomer(99);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCustomer_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            //Arrange
            var controller = new CustomersController(null);

            //Act
            var result = await controller.DeleteCustomer(4);

            //Assert
            Assert.Equal(500, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public async Task DeleteCustomer_ShouldReturnDeletedCustomerDetails_WhenCustomerIsDeleted()
        {
            //Arrange
            var controller = new CustomersController(_context);

            //Act
            var result = await controller.DeleteCustomer(3);
            var newResult = await controller.GetCustomer(3);

            //Assert
            Assert.Equal("Marilyn", ((Customer)((ObjectResult)result).Value).FirstName);
            Assert.Equal("Monroe", ((Customer)((ObjectResult)result).Value).LastName);

            Assert.IsType<NotFoundObjectResult>(newResult);
        }

        private void Seed(CustomersDBContext context)
        {
            var customers = DataGenerator.GetCustomerData();
            context.Customers.AddRange(customers);
            context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
