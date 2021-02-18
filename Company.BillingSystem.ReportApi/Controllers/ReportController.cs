using Company.BillingSystem.ReportApi.Managers;
using Company.BillingSystem.ReportApi.Models;
using Company.BillingSystem.ReportApi.Services;
using Company.BillingSystem.ReportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Company.BillingSystem.ReportApi.Controllers
{
    [ApiController]
    [Route("api/relatorios")]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;
        private IReportGeneratorSettings _reportGeneratorSettings;
        private IBillingApiService _billingApiService;
        private ICustomerApiService _customerApiService;


        public ReportController(ILogger<ReportController> logger, IReportGeneratorSettings reportGeneratorSettings, 
                                IBillingApiService billingApiService, ICustomerApiService customerApiService)
        {
            _logger = logger;
            _reportGeneratorSettings = reportGeneratorSettings;
            _billingApiService = billingApiService;
            _customerApiService = customerApiService;
        }

        [HttpGet(@"~/cobrancaporestados/mes/{month:int:range(1,12)}")]
        public async Task<StateBillingReport> GetStatesBillingByMonth(int month)
        {
            StateBillingReportManager stateBillingReportManager = new StateBillingReportManager(month, _reportGeneratorSettings, _billingApiService, _customerApiService);
            return await stateBillingReportManager.CreateReport();

        }
    }
}
