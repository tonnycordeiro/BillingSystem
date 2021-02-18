using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.BillingSystem.ReportApi.Models
{
    public interface IReportGeneratorSettings
    {
        int? MaxNumberOfConcurrentTasks { get; set; }
    }

    public class ReportGeneratorSettings : IReportGeneratorSettings
    {
        private int? _maxNumberOfConcurrentTasks;

        public int? MaxNumberOfConcurrentTasks
        {
            get => _maxNumberOfConcurrentTasks ?? Environment.ProcessorCount;
            set => _maxNumberOfConcurrentTasks = value;
        }
    }
}
