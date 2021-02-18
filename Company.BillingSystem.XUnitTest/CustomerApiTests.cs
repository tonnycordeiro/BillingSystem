using Company.BillingSystem.CustomerApi.Controllers;
using Company.BillingSystem.CustomerApi.Models;
using Company.BillingSystem.CustomerApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Company.BillingSystem.XUnitTest
{
    public class CustomerApiTests
    {
        [Fact]
        public async Task Post_shouldCreateBill()
        {
            Customer customer = new Customer()
            {
                Cpf = "83684303054",
                Name = "Heitor",
                State = "PR"
            };

            Moq.Mock<ICustomerService> mockCustomerService = new Moq.Mock<ICustomerService>();
            mockCustomerService.Setup(customerService => customerService.CreateAsync(It.IsAny<Customer>())).Returns(Task.FromResult(customer));
            var customerController = new CustomersController(mockCustomerService.Object);

            var response = await customerController.Create(customer.Name, customer.State, customer.Cpf);

            var actionResult = Assert.IsType<ActionResult<Customer>>(response);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<Customer>(createdAtActionResult.Value);
            Assert.Equal(customer.Cpf, returnValue.Id);
        }

        [Fact]
        public async Task Post_shouldReturnBadRequestWhenPersonNameIsEmpty()
        {
            Customer customer = new Customer()
            {
                Cpf = "83684303054",
                Name = String.Empty,
                State = "PR"
            };

            Moq.Mock<ICustomerService> mockCustomerService = new Moq.Mock<ICustomerService>();
            mockCustomerService.Setup(customerService => customerService.CreateAsync(It.IsAny<Customer>())).Returns(Task.FromResult(customer));
            var customerController = new CustomersController(mockCustomerService.Object);

            var response = await customerController.Create(customer.Name, customer.State, customer.Cpf);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task Post_shouldReturnBadRequestWhenStateIsEmpty()
        {
            Customer customer = new Customer()
            {
                Cpf = "83684303054",
                Name = "Ramires",
                State = String.Empty
            };

            Moq.Mock<ICustomerService> mockCustomerService = new Moq.Mock<ICustomerService>();
            mockCustomerService.Setup(customerService => customerService.CreateAsync(It.IsAny<Customer>())).Returns(Task.FromResult(customer));
            var customerController = new CustomersController(mockCustomerService.Object);

            var response = await customerController.Create(customer.Name, customer.State, customer.Cpf);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task Post_shouldReturnBadRequestWhenPersonIdIsEmpty()
        {
            Customer customer = new Customer()
            {
                Cpf = String.Empty,
                Name = "Ramires",
                State = "RR"
            };

            Moq.Mock<ICustomerService> mockCustomerService = new Moq.Mock<ICustomerService>();
            mockCustomerService.Setup(customerService => customerService.CreateAsync(It.IsAny<Customer>())).Returns(Task.FromResult(customer));
            var customerController = new CustomersController(mockCustomerService.Object);

            var response = await customerController.Create(customer.Name, customer.State, customer.Cpf);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task Post_shouldReturnBadRequestWhenPersonIdIsInvalid()
        {
            Customer customer = new Customer()
            {
                Cpf = "7868",
                Name = "Ramires",
                State = "RJ"
            };

            Moq.Mock<ICustomerService> mockCustomerService = new Moq.Mock<ICustomerService>();
            mockCustomerService.Setup(customerService => customerService.CreateAsync(It.IsAny<Customer>())).Returns(Task.FromResult(customer));
            var customerController = new CustomersController(mockCustomerService.Object);

            var response = await customerController.Create(customer.Name, customer.State, customer.Cpf);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task Post_shouldReturnBadRequestWhenPersonIdIsDuplicated()
        {
            Customer newCustomer = new Customer()
            {
                Cpf = "64985418064",
                Name = "Ramires",
                State = "RJ"
            };

            Customer alreadyStoredCustomer = new Customer()
            {
                Cpf = "64985418064",
                Name = "Ramires Santos",
                State = "SP"
            };

            Moq.Mock<ICustomerService> mockCustomerService = new Moq.Mock<ICustomerService>();
            mockCustomerService.Setup(customerService => customerService.GetAsync(It.IsAny<string>())).Returns(Task.FromResult(alreadyStoredCustomer));
            var customerController = new CustomersController(mockCustomerService.Object);

            var response = await customerController.Create(newCustomer.Name, newCustomer.State, newCustomer.Cpf);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        private async IAsyncEnumerable<string> GetIdListAsyncly(List<string> ids)
        {
            foreach (string id in ids)
                yield return id;

            await Task.CompletedTask;
        }

        [Fact]
        public async Task Get_shouldGetPersonIdList()
        {
            List<string> expectedIds = new List<string>()
            {
                "51417973099", "50947938028"
            };

            List<string> actualIds = new List<string>();

            Moq.Mock<ICustomerService> mockCustomerService = new Moq.Mock<ICustomerService>();
            mockCustomerService.Setup(
                customerService => customerService
                    .GetIdListAsync())
                    .Returns(() => GetIdListAsyncly(expectedIds));

            var customerController = new CustomersController(mockCustomerService.Object);

            var response = customerController.GetIdList();

            await foreach (string id in response)
            {
                actualIds.Add(id);
            }

            Assert.IsAssignableFrom<IAsyncEnumerable<string>>(response);
            Assert.Equal(expectedIds, actualIds);
        }

        [Fact]
        public async Task Get_shouldGetEmptyPersonIdList()
        {
            List<string> expectedIds = new List<string>();

            Moq.Mock<ICustomerService> mockCustomerService = new Moq.Mock<ICustomerService>();
            mockCustomerService.Setup(
                customerService => customerService
                    .GetIdListAsync())
                    .Returns(() => GetIdListAsyncly(expectedIds));

            var customerController = new CustomersController(mockCustomerService.Object);

            var response = customerController.GetIdList();

            int count = 0;
            await foreach (string id in response)
            {
                count++;
            }

            Assert.IsAssignableFrom<IAsyncEnumerable<string>>(response);
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task Get_shouldGetCustomerById()
        {
            Customer customer = new Customer()
            {
                Cpf = "64985418064",
                Name = "Ramires",
                State = "RJ"
            };

            Moq.Mock<ICustomerService> mockCustomerService = new Moq.Mock<ICustomerService>();
            mockCustomerService.Setup(customerService => customerService.GetAsync(It.IsAny<string>())).Returns(Task.FromResult(customer));
            var customerController = new CustomersController(mockCustomerService.Object);

            var response = await customerController.GetById(customer.Cpf);

            var okObjetResult = Assert.IsType<OkObjectResult>(response.Result);
            Assert.IsType<Customer>(okObjetResult.Value);
            Assert.Equal(customer, (okObjetResult.Value));
        }

        [Fact]
        public async Task Get_shouldReturnBadRequestWhenPersonIdIsInvalid()
        {
            Customer customer = new Customer()
            {
                Cpf = "1",
                Name = "Ramires",
                State = "RJ"
            };

            Moq.Mock<ICustomerService> mockCustomerService = new Moq.Mock<ICustomerService>();
            mockCustomerService.Setup(customerService => customerService.GetAsync(It.IsAny<string>())).Returns(Task.FromResult(customer));
            var customerController = new CustomersController(mockCustomerService.Object);

            var response = await customerController.GetById(customer.Cpf);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task Get_shouldReturnNotFoundWhenPersonIdNotExists()
        {
            Customer customer = null;
            string anyCpf = "04107332020";

            Moq.Mock<ICustomerService> mockCustomerService = new Moq.Mock<ICustomerService>();
            mockCustomerService.Setup(customerService => customerService.GetAsync(It.IsAny<string>())).Returns(Task.FromResult(customer));
            var customerController = new CustomersController(mockCustomerService.Object);

            var response = await customerController.GetById(anyCpf);

            Assert.IsType<NotFoundResult>(response.Result);
        }


    }
}
