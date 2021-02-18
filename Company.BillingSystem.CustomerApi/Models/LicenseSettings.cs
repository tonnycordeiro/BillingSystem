using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.BillingSystem.CustomerApi.Models
{
    public interface ILicenseSettings
    {
        string GoogleApplicationCredentialsPath { get; set; }
        string GoogleApplicationCredentialsKey { get; set; }
    }

    public class LicenseSettings : ILicenseSettings
    {
        public const string LICENSE_RELATIVE_PATH = @"\Licenses\API esp8266-226fe381758d.json";
        public const string GOOGLE_APPLICATION_CREDENTIALS_KEY = "GOOGLE_APPLICATION_CREDENTIALS";

        private string _googleApplicationCredentialsPath;
        private string _googleApplicationCredentialsKey;

        public string GoogleApplicationCredentialsPath { 
            get
            {
                if (!String.IsNullOrEmpty(_googleApplicationCredentialsPath))
                    return _googleApplicationCredentialsPath;

                string binPath = 
                        System.IO.Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace(@"file:\",String.Empty);

                return $"{binPath}{LICENSE_RELATIVE_PATH}";
            }
            set => _googleApplicationCredentialsPath = value; }

        public string GoogleApplicationCredentialsKey { 
            get => String.IsNullOrEmpty(_googleApplicationCredentialsKey)? GOOGLE_APPLICATION_CREDENTIALS_KEY : _googleApplicationCredentialsKey; 
            set => _googleApplicationCredentialsKey = value; 
        }
    }
}
