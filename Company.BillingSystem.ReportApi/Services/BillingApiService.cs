using Company.BillingSystem.ReportApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Company.BillingSystem.ReportApi.Services
{
    public interface IBillingApiService
    {
        Task<IEnumerable<Bill>> GetByMonthAsync(int month);
    }

    public class BillingApiService : IBillingApiService
    {
        private HttpClient _client;
        public BillingApiService(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient(BillingApiSettings.HTTP_CLIENT_NAME);
        }

        public async Task<IEnumerable<Bill>> GetByMonthAsync(int month)
        {
            var response = await _client.GetAsync($"/api/cobrancas?mes={month}")
                                        .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            return await System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<Bill>>(responseStream);
        }






    }
}
