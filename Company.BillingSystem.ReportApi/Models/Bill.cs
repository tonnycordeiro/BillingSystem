using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Company.BillingSystem.ReportApi.Models
{
    public class Bill
    {
        [JsonPropertyName("dueDate")]
        public DateTime DueDate { get; set; }

        [JsonPropertyName("personId")]
        public string PersonId { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }
    }
}
