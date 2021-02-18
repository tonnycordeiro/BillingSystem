using Company.BillingSystem.CustomerCalculatorService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Company.BillingSystem.CustomerCalculatorService.Services
{
    public interface IBillingApiService
    {
        void Add(Bill bill);
    }

    public class BillingApiService : IBillingApiService
    {
        private HttpClient _client;
        public BillingApiService(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient(BillingApiSettings.HTTP_CLIENT_NAME);
        }

        public void Add(Bill bill)
        {
            var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("dataVencimento", bill.DueDate.ToString("yyyy/MM/dd")),
                new KeyValuePair<string, string>("cpf", bill.PersonId),
                new KeyValuePair<string, string>("valorCobranca", bill.Value.ToString())
            };

            var res = _client.PostAsync($"/api/cobrancas", new FormUrlEncodedContent(values)).Result;
            res.EnsureSuccessStatusCode();
        }
    }
}
