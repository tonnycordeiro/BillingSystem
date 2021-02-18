using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.BillingSystem.CustomerCalculatorService.Models
{
    public class Bill
    {
        public DateTime DueDate { get; set; }

        public string PersonId { get; set; }

        public double Value { get; set; }
    }
}
