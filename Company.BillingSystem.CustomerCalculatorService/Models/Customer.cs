using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Company.BillingSystem.CustomerCalculatorService.Models
{
    public class Customer
    {
        public string Name { get; set; }

        public string State { get; set; }

        public string Cpf { get; set; }
    }
}
