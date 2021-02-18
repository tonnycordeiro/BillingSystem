using Company.BillingSystem.Common.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;


namespace Company.BillingSystem.XUnitTest
{
    public class CommonBusinessTests
    {

        [Fact]
        public void ShouldVerifyIfValidCpf()
        {
            CpfValidator cpfValidator = new CpfValidator();
            Assert.True(cpfValidator.IsValid("907.518.060-80"));
        }

        [Fact]
        public void ShouldVerifyIfInvalidCpf()
        {
            CpfValidator cpfValidator = new CpfValidator();
            Assert.False(cpfValidator.IsValid("907.518.060-8"));
        }

        [Fact]
        public void ShouldVerifyEmptyCpfAsInvalid()
        {
            CpfValidator cpfValidator = new CpfValidator();
            Assert.False(cpfValidator.IsValid(String.Empty));
        }

        [Fact]
        public void ShouldVerifyNullCpfAsInvalid()
        {
            CpfValidator cpfValidator = new CpfValidator();
            Assert.False(cpfValidator.IsValid(null));
        }

        [Fact]
        public void ShouldFormatCpfWithoutNumberSeparator()
        {
            string cpf = "706.534.060-02";
            string expectedFormatedCpf = "706534060-02";
            
            CpfValidator cpfValidator = new CpfValidator();
            Assert.Equal(cpfValidator.Format(cpf, CodeFormat.WithoutNumberSeparator), expectedFormatedCpf);
        }

        [Fact]
        public void ShouldFormatCpfWithoutDigitSeparator()
        {
            string cpf = "706.534.060-02";
            string expectedFormatedCpf = "706.534.06002";

            CpfValidator cpfValidator = new CpfValidator();
            Assert.Equal(cpfValidator.Format(cpf, CodeFormat.WithoutDigitSeparator), expectedFormatedCpf);
        }

        [Fact]
        public void ShouldFormatCpfWithoutDigit()
        {
            string cpf = "706.534.060-02";
            string expectedFormatedCpf = "706.534.060";

            CpfValidator cpfValidator = new CpfValidator();
            Assert.Equal(cpfValidator.Format(cpf, CodeFormat.WithoutDigit), expectedFormatedCpf);
        }


        [Fact]
        public void ShouldFormatCpfWithoutSeparators()
        {
            string cpf = "706.534.060-02";
            string expectedFormatedCpf = "70653406002";

            CpfValidator cpfValidator = new CpfValidator();
            Assert.Equal(cpfValidator.Format(cpf, CodeFormat.WithoutNumberSeparator | CodeFormat.WithoutDigitSeparator), expectedFormatedCpf);
        }

        [Fact]
        public void ShouldTrimCpf()
        {
            string cpf = "    706.534.060-02   ";
            string expectedFormatedCpf = "706.534.060-02";

            CpfValidator cpfValidator = new CpfValidator();
            Assert.Equal(cpfValidator.Format(cpf, CodeFormat.Trim), expectedFormatedCpf);
        }

    }
}
