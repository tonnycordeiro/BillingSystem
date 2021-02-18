using Company.BillingSystem.CustomerCalculatorService.Models;
using Company.BillingSystem.CustomerCalculatorService.Providers;
using Company.BillingSystem.CustomerCalculatorService.Services;
using Company.BillingSystem.CustomerCalculatorService.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Company.BillingSystem.CustomerCalculatorService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigProvider configProvider = new ConfigProvider();
            CreateHostBuilder(args, configProvider).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, ConfigProvider configProvider) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<CustomerBillingGeneratorSettings>(
                        configProvider.Configuration.GetSection(nameof(CustomerBillingGeneratorSettings)));

                    services.AddSingleton<ICustomerBillingGeneratorSettings>(
                        sp => sp.GetRequiredService<IOptions<CustomerBillingGeneratorSettings>>().Value);

                    services.AddHostedService<CustomerBillingGeneratorWorker>();

                    services.AddSingleton<IBillingApiService>(
                        sp => new BillingApiService(sp.GetRequiredService<IHttpClientFactory>()));

                    services.AddSingleton<ICustomerApiService>(
                        sp => new CustomerApiService(sp.GetRequiredService<IHttpClientFactory>()));

                    services.AddHttpClient(CustomerApiSettings.HTTP_CLIENT_NAME, c =>
                    {
                        c.BaseAddress = new Uri(configProvider.CustomerApiSettings.BaseAddress);
                        c.DefaultRequestHeaders.Accept.Clear();
                        c.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
                    });
                    services.AddHttpClient(BillingApiSettings.HTTP_CLIENT_NAME, c =>
                    {
                        c.BaseAddress = new Uri(configProvider.BillingApiSettings.BaseAddress);
                        c.DefaultRequestHeaders.Accept.Clear();
                        c.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
                    });
                });

    }
}
