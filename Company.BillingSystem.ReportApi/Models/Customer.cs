using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Company.BillingSystem.ReportApi.Models
{
    public class Customer
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }
    }
}
