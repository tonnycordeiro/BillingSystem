using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.BillingSystem.ReportApi.Models
{
    public class BillingApiSettings : IBillingApiSettings
    {
        public const string HTTP_CLIENT_NAME = "BillingApi";

        public string BaseAddress { get; set; }
    }
    public interface IBillingApiSettings
    {
        string BaseAddress { get; set; }
    }
}
