using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Company.BillingSystem.ReportApi.Models
{
    
    public class StateBilling
    {
        [JsonPropertyName("Estado")]
        public string State { get; set; }
        
        [JsonPropertyName("Valor")]
        public double Value { get; set; }
    }
}
