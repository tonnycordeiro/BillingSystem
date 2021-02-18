using Company.BillingSystem.CustomerCalculatorService.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.BillingSystem.CustomerCalculatorService.Providers
{
    public class ConfigProvider
    {
        public const string APP_SETTINGS_JSON_PATH = "./appsettings.json";

        public IConfigurationRoot Configuration { get; set; }

        public ConfigProvider(string appSettingJsonPath = APP_SETTINGS_JSON_PATH)
        {
            var cfgBuilder = new ConfigurationBuilder().AddJsonFile(appSettingJsonPath);
            Configuration = cfgBuilder.Build();
        }

        public ICustomerApiSettings CustomerApiSettings 
        { 
            get 
            {
                return new CustomerApiSettings()
                {
                    BaseAddress = Configuration["CustomerApiSettings:BaseAddress"]
                };
            }
        }

        public IBillingApiSettings BillingApiSettings
        {
            get
            {
                return new BillingApiSettings()
                {
                    BaseAddress = Configuration["BillingApiSettings:BaseAddress"]
                };
            }
        }
    }
}
