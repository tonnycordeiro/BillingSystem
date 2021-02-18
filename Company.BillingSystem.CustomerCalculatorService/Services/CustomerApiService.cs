using Company.BillingSystem.CustomerCalculatorService.Models;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;


namespace Company.BillingSystem.CustomerCalculatorService.Services
{
    public interface ICustomerApiService
    {
        Task<IEnumerable<string>> GetAllIdsAsync();
        Task<Customer> GetCustomerAsync(string cpf);
    }

    public class CustomerApiService : ICustomerApiService
    {
        private HttpClient _client;

        public CustomerApiService(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient(CustomerApiSettings.HTTP_CLIENT_NAME);
        }

        public async Task<Customer> GetCustomerAsync(string cpf)
        {
            var response = await _client.GetAsync($"/api/clientes/{cpf}")
                                        .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var customer = JsonConvert.DeserializeObject<Customer>(await response.Content.ReadAsStringAsync());

            return customer;
        }

        public async Task<IEnumerable<string>> GetAllIdsAsync()
        {
            var response = await _client.GetAsync($"/api/clientes/cpfs")
                                        .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            return await System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<string>>(responseStream);
        }

    }
}
