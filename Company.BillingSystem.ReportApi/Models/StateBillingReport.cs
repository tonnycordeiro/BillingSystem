using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Company.BillingSystem.ReportApi.Models
{
    public class StateBillingReport
    {
        [JsonPropertyName("mês")]
        public int Month { get; set; }

        public IDictionary<string, double> StateByValueDic;

        [JsonPropertyName("cobranças")]
        public IEnumerable<StateBilling> StateBillingList
        { 
            get
            {
                return StateByValueDic.ToList().Select(pair => new StateBilling() {State = pair.Key, Value = pair.Value });
            }
        }

    }
}
