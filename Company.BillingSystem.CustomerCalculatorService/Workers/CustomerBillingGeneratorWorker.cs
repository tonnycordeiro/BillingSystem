using Company.BillingSystem.CustomerCalculatorService.Services;
using Company.BillingSystem.CustomerCalculatorService.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Company.BillingSystem.CustomerCalculatorService.Workers
{
    public class CustomerBillingGeneratorWorker : BackgroundService
    {
        private readonly ILogger<CustomerBillingGeneratorWorker> _logger;
        private readonly ICustomerBillingGeneratorSettings _customerBillingGeneratorSettings;
        private IBillingApiService _billingApiService;
        private ICustomerApiService _customerApiService;

        public CustomerBillingGeneratorWorker(
            ILogger<CustomerBillingGeneratorWorker> logger,
            ICustomerBillingGeneratorSettings customerBillingGeneratorSettings, 
            IBillingApiService billingApiService, ICustomerApiService customerApiService)
        {
            _logger = logger;
            _customerBillingGeneratorSettings = customerBillingGeneratorSettings;
            _customerApiService = customerApiService;
            _billingApiService = billingApiService;
        }

        /// <summary>
        /// This generate customer billing through "Producer Consumer" pattern, with parallelization.
        /// The maximum number of threads is equivalent to processors count (default), but can be customized in appsettings.json
        /// The amount of days to calculate the due date from today is also customized.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var blockingCollection = new BlockingCollection<string>(_customerBillingGeneratorSettings.BoundedCapacityForBlockingCollection.Value);

            Action<string> billCalculatingWork = cpf =>
            {
                double value = Double.Parse($"{cpf.Substring(0,2)}{cpf.Substring(cpf.Length-2, 2)}");
                _billingApiService.Add(new Bill() { DueDate = DateTime.Now.AddDays(_customerBillingGeneratorSettings.AmountOfDaysToDueDate.Value),
                                                PersonId = cpf, Value = value });
            };

            _ = Task.Run(async() =>
            {
                IEnumerable<string> customerIds = await _customerApiService.GetAllIdsAsync();

                foreach(string id in customerIds.ToList())
                {
                    blockingCollection.Add(id);
                }

                blockingCollection.CompleteAdding();
            });

            blockingCollection
                .GetConsumingEnumerable()
                .AsParallel()
                .WithDegreeOfParallelism(_customerBillingGeneratorSettings.MaxNumberOfConcurrentTasks.Value)
                .WithMergeOptions(ParallelMergeOptions.NotBuffered)
                .ForAll(billCalculatingWork);

            await Task.CompletedTask;
        }
    }
}
