using System;
using System.Collections.Generic;
using System.Text;

namespace Company.BillingSystem.Common.Validators
{
    public class CpfValidator
    {
        public const string NUMBER_SEPARATOR = ".";
        public const string DIGIT_SEPARATOR = "-";

        public bool IsValid(string value)
        {
            int numberOfDigits1 = 10;
            int numberOfDigits2 = 11;
            string temp;
            string digit;

            if (String.IsNullOrEmpty(value))
                return false;

            string code = Format(value, CodeFormat.Trim | CodeFormat.WithoutDigitSeparator | CodeFormat.WithoutNumberSeparator);

            if (code.Length != 11)
                return false;

            temp = code.Substring(0, 9);
            digit = GetDigit(temp, numberOfDigits1);
            temp = $"{temp}{digit}";

            digit = $"{digit}{GetDigit(temp, numberOfDigits2)}";

            return value.EndsWith(digit);
        }

        private string GetDigit(string value, int numberOfDigits)
        {
            int sum = 0;
            int rest;

            for (int i = 0; numberOfDigits >= 2; i++)
            {
                sum += (int.Parse(value[i].ToString()) * numberOfDigits--);
            }

            rest = (sum % 11);
            rest = rest < 2 ? 0 : (11 - rest);

            return rest.ToString();
        }


        public string Format(string value, CodeFormat codeFormat)
        {
            string code = value;

            try
            {
                if ((codeFormat & CodeFormat.Trim) == CodeFormat.Trim)
                    code = code.Trim();

                if ((codeFormat & CodeFormat.WithoutNumberSeparator) == CodeFormat.WithoutNumberSeparator)
                    code = code.Replace(NUMBER_SEPARATOR, "");

                if ((codeFormat & CodeFormat.WithoutDigit) == CodeFormat.WithoutDigit)
                {
                    string digit = value.Trim().Substring(value.Length - 2, 2);
                    code = code.Substring(0, code.LastIndexOf(digit));

                    codeFormat |= CodeFormat.WithoutDigitSeparator;
                }

                if ((codeFormat & CodeFormat.WithoutDigitSeparator) == CodeFormat.WithoutDigitSeparator)
                    code = code.Replace(DIGIT_SEPARATOR, "");
            }
            catch
            {
                throw;
            }

            return code;
        }
    }
}
