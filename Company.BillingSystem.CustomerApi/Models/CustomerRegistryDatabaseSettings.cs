using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.BillingSystem.CustomerApi.Models
{
    public class CustomerRegistryDatabaseSettings : ICustomerRegistryDatabaseSettings
    {
        public string ProjectId { get; set; }
        public string CustomersCollectionName { get; set; }
    }
    public interface ICustomerRegistryDatabaseSettings
    {
        string ProjectId { get; set; }
        string CustomersCollectionName { get; set; }
    }
}
