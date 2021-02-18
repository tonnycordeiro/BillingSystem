using Company.BillingSystem.CustomerCalculatorService.Models;
using Company.BillingSystem.CustomerCalculatorService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Company.BillingSystem.CustomerCalculatorService.Workers;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Company.BillingSystem.XUnitTest
{
    public class CustomerCalculatorServiceTests
    {

        [Fact]
        public void ShouldValidateBillingCalculation()
        {
            bool isCalculatedRight = true;
            List<Bill> actualBillingList = new List<Bill>();
            IEnumerable<string> cpfList = new List<string>() { "70653406002", "37356980002", "78521937067", "51978161042", "71945176040" };
            List<Bill> expectedBillingList = new List<Bill>();

            Moq.Mock<IBillingApiService> mockBillingApiService = new Moq.Mock<IBillingApiService>();
            mockBillingApiService.Setup(billsService => billsService.Add(It.IsAny<Bill>()))
                .Callback<Bill>(bill => actualBillingList.Add(bill));

            Moq.Mock<ICustomerApiService> mockCustomerApiService = new Moq.Mock<ICustomerApiService>();
            mockCustomerApiService.Setup(customersService => customersService.GetAllIdsAsync()).Returns(Task.FromResult(cpfList));

            mockCustomerApiService.Setup(customersService => customersService.GetCustomerAsync(It.IsAny<string>()))
                .ReturnsAsync((string personId) => new Customer() {Cpf = personId, Name = String.Empty, State = "SP" });

            Moq.Mock<ILogger<CustomerBillingGeneratorWorker>> mockLogger = new Moq.Mock<ILogger<CustomerBillingGeneratorWorker>>();

            CustomerBillingGeneratorSettings settings = new CustomerBillingGeneratorSettings()
            {
                AmountOfDaysToDueDate = 10,
                MaxNumberOfConcurrentTasks = 4,
                BoundedCapacityForBlockingCollection = 100
            };

            CustomerBillingGeneratorWorker worker = new CustomerBillingGeneratorWorker(mockLogger.Object, settings,
                mockBillingApiService.Object, mockCustomerApiService.Object);

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            Task.WaitAll(worker.StartAsync(token));

            foreach (Bill bill in actualBillingList)
            {
                double value = Double.Parse($"{bill.PersonId.Substring(0, 2)}{bill.PersonId.Substring(bill.PersonId.Length - 2, 2)}");
                if(value != bill.Value)
                {
                    isCalculatedRight = false;
                    break;
                }
            }

            Assert.Empty(actualBillingList.Select(b => b.PersonId).Except(cpfList));
            Assert.True(isCalculatedRight);
        }

        [Fact]
        public void ShouldValidateDueDateCalculation()
        {
            bool isCalculatedRight = true;
            List<Bill> actualBillingList = new List<Bill>();
            IEnumerable<string> cpfList = new List<string>() { "70653406002", "37356980002", "78521937067", "51978161042", "71945176040" };
            List<Bill> expectedBillingList = new List<Bill>();
            
            int amountOfDaysToDueDate = 10;
            DateTime now = DateTime.Now;

            Moq.Mock<IBillingApiService> mockBillingApiService = new Moq.Mock<IBillingApiService>();
            mockBillingApiService.Setup(billsService => billsService.Add(It.IsAny<Bill>()))
                .Callback<Bill>(bill => actualBillingList.Add(bill));

            Moq.Mock<ICustomerApiService> mockCustomerApiService = new Moq.Mock<ICustomerApiService>();
            mockCustomerApiService.Setup(customersService => customersService.GetAllIdsAsync()).Returns(Task.FromResult(cpfList));

            mockCustomerApiService.Setup(customersService => customersService.GetCustomerAsync(It.IsAny<string>()))
                .ReturnsAsync((string personId) => new Customer() { Cpf = personId, Name = String.Empty, State = "SP" });

            Moq.Mock<ILogger<CustomerBillingGeneratorWorker>> mockLogger = new Moq.Mock<ILogger<CustomerBillingGeneratorWorker>>();

            CustomerBillingGeneratorSettings settings = new CustomerBillingGeneratorSettings()
            {
                AmountOfDaysToDueDate = amountOfDaysToDueDate,
                MaxNumberOfConcurrentTasks = 4,
                BoundedCapacityForBlockingCollection = 100
            };

            CustomerBillingGeneratorWorker worker = new CustomerBillingGeneratorWorker(mockLogger.Object, settings,
                mockBillingApiService.Object, mockCustomerApiService.Object);

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            Task.WaitAll(worker.StartAsync(token));

            foreach (Bill bill in actualBillingList)
            {
                double value = Double.Parse($"{bill.PersonId.Substring(0, 2)}{bill.PersonId.Substring(bill.PersonId.Length - 2, 2)}");
                if (bill.DueDate.AddDays(-amountOfDaysToDueDate).CompareTo(now) <0)
                {
                    isCalculatedRight = false;
                    break;
                }
            }

            Assert.Empty(actualBillingList.Select(b => b.PersonId).Except(cpfList));
            Assert.True(isCalculatedRight);
        }

        [Fact]
        public void ShouldValidatePerformanceWith4Threads()
        {
            List<Bill> actualBillingList = new List<Bill>();
            IEnumerable<string> cpfList = new List<string>() { "70653406002", "37356980002", "78521937067", "51978161042", "71945176040" };
            List<Bill> expectedBillingList = new List<Bill>();
            
            int numberOfThreads = 4;
            DateTime startTime;
            DateTime endTime;

            Moq.Mock<IBillingApiService> mockBillingApiService = new Moq.Mock<IBillingApiService>();
            mockBillingApiService.Setup(billsService => billsService.Add(It.IsAny<Bill>()))
                .Callback<Bill>(bill => actualBillingList.Add(bill));

            Moq.Mock<ICustomerApiService> mockCustomerApiService = new Moq.Mock<ICustomerApiService>();
            mockCustomerApiService.Setup(customersService => customersService.GetAllIdsAsync()).Returns(Task.FromResult(cpfList));

            mockCustomerApiService.Setup(customersService => customersService.GetCustomerAsync(It.IsAny<string>()))
                .ReturnsAsync((string personId) => new Customer() { Cpf = personId, Name = String.Empty, State = "SP" });

            Moq.Mock<ILogger<CustomerBillingGeneratorWorker>> mockLogger = new Moq.Mock<ILogger<CustomerBillingGeneratorWorker>>();

            CustomerBillingGeneratorSettings settings = new CustomerBillingGeneratorSettings()
            {
                AmountOfDaysToDueDate = 10,
                MaxNumberOfConcurrentTasks = numberOfThreads,
                BoundedCapacityForBlockingCollection = 100
            };

            CustomerBillingGeneratorWorker worker = new CustomerBillingGeneratorWorker(mockLogger.Object, settings,
                mockBillingApiService.Object, mockCustomerApiService.Object);

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            startTime = DateTime.Now;
            Task.WaitAll(worker.StartAsync(token));
            endTime = DateTime.Now;

            double expectedTiming = 300;
            double actualTiming = (int)(endTime - startTime).TotalMilliseconds;

            Assert.True(actualTiming <= expectedTiming, $"Actual timing is {actualTiming} milliseconds. Expected {expectedTiming} milliseconds");
        }
    }
}
