using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.BillingSystem.ReportApi.Models
{
    public interface ICustomerApiSettings
    {
        string BaseAddress { get; set; }
    }

    public class CustomerApiSettings : ICustomerApiSettings
    {
        public const string HTTP_CLIENT_NAME = "CustomerApi";
        public string BaseAddress { get; set; }
    }
}
