using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.BillingSystem.BillingApi.Models
{
    public class BillingDatabaseSettings : IBillingDatabaseSettings
    {
        public string BillsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IBillingDatabaseSettings
    {
        string BillsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
