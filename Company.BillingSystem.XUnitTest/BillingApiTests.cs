using Company.BillingSystem.BillingApi.Controllers;
using Company.BillingSystem.BillingApi.Models;
using Company.BillingSystem.BillingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Company.BillingSystem.XUnitTest
{
    public class BillingApiTests
    {
        [Fact]
        public async Task Post_shouldCreateBill()
        {
            Bill bill = new Bill()
            {
                DueDate = DateTime.Now,
                Id = "anykey1",
                PersonId = "81309409064",
                Value = 22
            };

            Moq.Mock<IBillService> mockBillService = new Moq.Mock<IBillService>();
            mockBillService.Setup(billService => billService.CreateAsync(It.IsAny<Bill>())).ReturnsAsync(bill);
            var billsController = new BillsController(mockBillService.Object);

            var response = await billsController.Create(bill.DueDate, bill.PersonId, bill.Value);

            var okObjetResult = Assert.IsType<OkObjectResult>(response.Result);
            Assert.IsType<Bill>(okObjetResult.Value);
            Assert.Equal(bill, (okObjetResult.Value));
        }

        [Fact]
        public async Task Post_shouldReturnBadRequestWhenPersonIdIsInvalid()
        {
            Bill bill = new Bill()
            {
                DueDate = DateTime.Now,
                Id = "anykey1",
                PersonId = "464612",
                Value = 22
            };

            Moq.Mock<IBillService> mockBillService = new Moq.Mock<IBillService>();
            mockBillService.Setup(billService => billService.CreateAsync(It.IsAny<Bill>())).ReturnsAsync(bill);
            var billsController = new BillsController(mockBillService.Object);

            var response = await billsController.Create(bill.DueDate, bill.PersonId, bill.Value);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task Post_shouldReturnBadRequestWhenPersonIdIsEmpty()
        {
            Bill bill = new Bill()
            {
                DueDate = DateTime.Now,
                Id = "anykey1",
                PersonId = String.Empty,
                Value = 22
            };

            Moq.Mock<IBillService> mockBillService = new Moq.Mock<IBillService>();
            mockBillService.Setup(billService => billService.CreateAsync(It.IsAny<Bill>())).ReturnsAsync(bill);
            var billsController = new BillsController(mockBillService.Object);

            var response = await billsController.Create(bill.DueDate, bill.PersonId, bill.Value);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        private async IAsyncEnumerable<Bill> GetBillListAsyncly(List<Bill> bills)
        {
            foreach(Bill bill  in bills)
                yield return bill;

            await Task.CompletedTask;
        }

        [Fact]
        public async Task Get_shouldTakeEmptyBillList()
        {
            List<Bill> bills = new List<Bill>()
            {
                new Bill { DueDate = DateTime.Now.AddDays(3), PersonId = "51417973099", Value = 20 },
                new Bill { DueDate = DateTime.Now.AddDays(3), PersonId = "50947938028", Value = 22 }
            };

            Moq.Mock<IBillService> mockBillService = new Moq.Mock<IBillService>();
            mockBillService.Setup(
                billService => billService
                    .GetAsync(It.IsAny<string>(), It.IsAny<int?>()))
                    .Returns(() => GetBillListAsyncly(bills));

            var billsController = new BillsController(mockBillService.Object);

            

            var response = billsController.GetBills(null, null);

            int count = 0;
            await foreach (Bill b in response)
            {
                count++;
            }

            Assert.IsAssignableFrom<IAsyncEnumerable<Bill>>(response);
            Assert.Equal(0, count);
        }


        [Fact]
        public async Task Get_shouldTakeBillsByMonth()
        {
            List<Bill> expectedBills = new List<Bill>()
            {
                new Bill { DueDate = DateTime.Now.AddDays(3), PersonId = "51417973099", Value = 20 },
                new Bill { DueDate = DateTime.Now.AddDays(3), PersonId = "50947938028", Value = 22 }
            };

            List<Bill> actualBills = new List<Bill>();

            Moq.Mock<IBillService> mockBillService = new Moq.Mock<IBillService>();
            mockBillService.Setup(
                billService => billService
                    .GetAsync(It.IsAny<string>(), It.IsAny<int?>()))
                    .Returns(() => GetBillListAsyncly(expectedBills));

            var billsController = new BillsController(mockBillService.Object);

            await foreach (Bill b in billsController.GetBills(mes: 1))
            {
                actualBills.Add(b);
            }

            Assert.Equal(expectedBills, actualBills);
        }

        [Fact]
        public async Task Get_shouldTakeBillsByPersonId()
        {
            List<Bill> expectedBills = new List<Bill>()
            {
                new Bill { DueDate = DateTime.Now.AddDays(3), PersonId = "51417973099", Value = 20 }
            };

            List<Bill> actualBills = new List<Bill>();

            Moq.Mock<IBillService> mockBillService = new Moq.Mock<IBillService>();
            mockBillService.Setup(
                billService => billService
                    .GetAsync(It.IsAny<string>(), It.IsAny<int?>()))
                    .Returns(() => GetBillListAsyncly(expectedBills));

            var billsController = new BillsController(mockBillService.Object);

            await foreach (Bill b in billsController.GetBills(cpf: "51417973099"))
            {
                actualBills.Add(b);
            }

            Assert.Equal(expectedBills, actualBills);
        }


    }
}
