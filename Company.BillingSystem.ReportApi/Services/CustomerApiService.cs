using Company.BillingSystem.ReportApi.Models;
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

namespace Company.BillingSystem.ReportApi.Services
{
    public interface ICustomerApiService
    {
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
            if (String.IsNullOrEmpty(cpf))
                return null;

            var response = await _client.GetAsync($"/api/clientes/{cpf}")
                                        .ConfigureAwait(true);

            if (response.IsSuccessStatusCode)
            {
                var customer = JsonConvert.DeserializeObject<Customer>(await response.Content.ReadAsStringAsync());
                return customer;
            }

            else
                return null;
        }


    }
}
