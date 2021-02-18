using System;
using System.Collections.Generic;
using System.Text;

namespace Company.BillingSystem.Common.Validators
{
    [Flags]
    public enum CodeFormat
    {
        WithoutNumberSeparator = 1,
        WithoutDigitSeparator = 2,
        WithoutDigit = 4,
        Trim = 8
    }
}
