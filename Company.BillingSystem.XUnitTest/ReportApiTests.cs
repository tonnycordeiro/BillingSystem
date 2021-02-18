using Company.BillingSystem.ReportApi.Controllers;
using Company.BillingSystem.ReportApi.Managers;
using Company.BillingSystem.ReportApi.Models;
using Company.BillingSystem.ReportApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace Company.BillingSystem.XUnitTest
{
    public class ReportApiTests
    {
        [Fact]
        public async Task Get_ShouldValidateBillValueByState()
        {
            bool isCalculatedRight = true;

            List<Bill> bills = new List<Bill>()
            {
                new Bill() {DueDate = new DateTime(2021,03,15), PersonId = "89971312069", Value = 33 },
                new Bill() {DueDate = new DateTime(2021,03,15), PersonId = "30254863094", Value = 44 },
                new Bill() {DueDate = new DateTime(2021,03,15), PersonId = "89971312069", Value = 55 },
                new Bill() {DueDate = new DateTime(2021,03,15), PersonId = "30254863094", Value = 78.4 },
                new Bill() {DueDate = new DateTime(2021,03,15), PersonId = "89971312069", Value = 89.3 },
                new Bill() {DueDate = new DateTime(2021,03,15), PersonId = "88873823017", Value = 1 },
            };

            Dictionary<string, string> personIdToState = new Dictionary<string, string>()
            {
                {"89971312069", "SP" },
                {"30254863094", "RJ" },
                {"88873823017", "SP" }
            };

            int month = 3;

            Moq.Mock<IBillingApiService> mockBillingApiService = new Moq.Mock<IBillingApiService>();
            mockBillingApiService.Setup(billsService => billsService.GetByMonthAsync(It.IsAny<int>()))
                .ReturnsAsync(bills);

            Moq.Mock<ICustomerApiService> mockCustomerApiService = new Moq.Mock<ICustomerApiService>();
            mockCustomerApiService.Setup(customersService => customersService.GetCustomerAsync(It.IsAny<string>()))
                .ReturnsAsync((string personId) => new Customer() { Cpf = personId, Name = String.Empty, 
                                                                    State = personIdToState[personId] });

            Moq.Mock<ILogger<ReportController>> mockLogger = new Moq.Mock<ILogger<ReportController>>();

            ReportGeneratorSettings reportGeneratorSettings = new ReportGeneratorSettings()
            {
                MaxNumberOfConcurrentTasks = 4
            };

            ReportController reportController = new ReportController(mockLogger.Object, reportGeneratorSettings,
                mockBillingApiService.Object, mockCustomerApiService.Object);

            StateBillingReport response = await reportController.GetStatesBillingByMonth(month);

            
            foreach(StateBilling stateBilling in response.StateBillingList)
            {

                if(bills.Where(b => b.DueDate.Month == month && personIdToState[b.PersonId] == stateBilling.State)
                     .Sum(b => b.Value) != stateBilling.Value)
                {
                    isCalculatedRight = false;
                    break;
                }
            }

            Assert.True(isCalculatedRight);
        }

        [Fact]
        public async Task Get_ShouldValidatePerformanceWith4Threads()
        {
            List<Bill> bills = new List<Bill>()
            {
                new Bill() {DueDate = new DateTime(2021,03,15), PersonId = "89971312069", Value = 33 },
                new Bill() {DueDate = new DateTime(2021,03,15), PersonId = "30254863094", Value = 44 },
                new Bill() {DueDate = new DateTime(2021,03,15), PersonId = "89971312069", Value = 55 },
                new Bill() {DueDate = new DateTime(2021,03,15), PersonId = "30254863094", Value = 78.4 },
                new Bill() {DueDate = new DateTime(2021,03,15), PersonId = "89971312069", Value = 89.3 },
                new Bill() {DueDate = new DateTime(2021,03,15), PersonId = "88873823017", Value = 1 },
            };

            Dictionary<string, string> personIdToState = new Dictionary<string, string>()
            {
                {"89971312069", "SP" },
                {"30254863094", "RJ" },
                {"88873823017", "SP" }
            };

            int numberOfThreads = 4;
            DateTime startTime;
            DateTime endTime;
            int month = 3;

            Moq.Mock<IBillingApiService> mockBillingApiService = new Moq.Mock<IBillingApiService>();
            mockBillingApiService.Setup(billsService => billsService.GetByMonthAsync(It.IsAny<int>()))
                .ReturnsAsync(bills);

            Moq.Mock<ICustomerApiService> mockCustomerApiService = new Moq.Mock<ICustomerApiService>();
            mockCustomerApiService.Setup(customersService => customersService.GetCustomerAsync(It.IsAny<string>()))
                .ReturnsAsync((string personId) => new Customer()
                {
                    Cpf = personId,
                    Name = String.Empty,
                    State = personIdToState[personId]
                });

            Moq.Mock<ILogger<ReportController>> mockLogger = new Moq.Mock<ILogger<ReportController>>();

            ReportGeneratorSettings reportGeneratorSettings = new ReportGeneratorSettings()
            {
                MaxNumberOfConcurrentTasks = numberOfThreads
            };

            ReportController reportController = new ReportController(mockLogger.Object, reportGeneratorSettings,
                mockBillingApiService.Object, mockCustomerApiService.Object);

            startTime = DateTime.Now;
            StateBillingReport response = await reportController.GetStatesBillingByMonth(month);
            endTime = DateTime.Now;

            double expectedTiming = 350;
            double actualTiming = (int)(endTime - startTime).TotalMilliseconds;

            Assert.True(actualTiming <= expectedTiming, $"Actual timing is {actualTiming} milliseconds. Expected {expectedTiming} milliseconds");
        }

    }
}
