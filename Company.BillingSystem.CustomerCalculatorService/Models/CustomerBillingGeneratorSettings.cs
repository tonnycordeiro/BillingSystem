using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.BillingSystem.CustomerCalculatorService.Models
{
    public interface ICustomerBillingGeneratorSettings
    {
        int? AmountOfDaysToDueDate { get; set; }
        int? BoundedCapacityForBlockingCollection { get; set; }
        int? MaxNumberOfConcurrentTasks { get; set; }
    }

    public class CustomerBillingGeneratorSettings : ICustomerBillingGeneratorSettings
    {
        public const int BOUNDED_CAPACITY_FOR_BLOCKING_COLLECTION_DEFAULT = 1000;
        public const int AMOUNT_OF_DAYS_TO_DUE_DATE = 30;

        private int? _amountOfDaysToDueDate;
        private int? _boundedCapacityForBlockingCollection;
        private int? _maxNumberOfConcurrentTasks;

        public int? AmountOfDaysToDueDate { 
                get => _amountOfDaysToDueDate ?? AMOUNT_OF_DAYS_TO_DUE_DATE;
                set => _amountOfDaysToDueDate = value; 
        }
        public int? BoundedCapacityForBlockingCollection { 
                get => _boundedCapacityForBlockingCollection ?? BOUNDED_CAPACITY_FOR_BLOCKING_COLLECTION_DEFAULT;
                set => _boundedCapacityForBlockingCollection = value;
        }
        public int? MaxNumberOfConcurrentTasks { 
                get => _maxNumberOfConcurrentTasks ?? Environment.ProcessorCount;
                set => _maxNumberOfConcurrentTasks = value;
        }
    }
}
