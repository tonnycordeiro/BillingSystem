using Company.BillingSystem.CustomerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.BillingSystem.CustomerApi.Managers
{
    public class LicenseManager
    {
        public const string LICENSE_RELATIVE_PATH = @"\Licenses\API esp8266-226fe381758d.json";
        public const string GOOGLE_APPLICATION_CREDENTIALS_KEY = "GOOGLE_APPLICATION_CREDENTIALS";
        private ILicenseSettings _licenseSettings;

        public LicenseManager(ILicenseSettings licenseSettings)
        {
            _licenseSettings = licenseSettings;
        }

        /// <summary>
        /// It is defined the environment variable to find the file's path with service account data to authenticate Firebase connection.
        /// This file can be inside (default) or outside (customized in appsettings.json) of the project.
        /// </summary>
        public void ApplyLicenses()
        {
            Environment.SetEnvironmentVariable(_licenseSettings.GoogleApplicationCredentialsKey,
                                               _licenseSettings.GoogleApplicationCredentialsPath);
        }

    }
}
