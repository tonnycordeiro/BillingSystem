using Company.BillingSystem.BillingApi.Models;
using Company.BillingSystem.BillingApi.Services;
using Company.BillingSystem.Common.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.BillingSystem.BillingApi.Controllers
{

    [Route("api/cobrancas")]
    [ApiController]
    public class BillsController : ControllerBase
    {
        private readonly IBillService _billService;

        public BillsController(IBillService billService)
        {
            _billService = billService;
        }

        [HttpGet]
        public async IAsyncEnumerable<Bill> GetBills(string cpf = null, int? mes = null)
        {
            if (!String.IsNullOrEmpty(cpf) || mes.HasValue)
            {
                await foreach (Bill bill in _billService.GetAsync(cpf, mes))
                {
                    yield return bill;
                }
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Bill))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Bill>> Create([FromForm] DateTime dataVencimento, [FromForm] string cpf, [FromForm] double valorCobranca)
        {
            CpfValidator cpfValidator = new CpfValidator();

            if (!cpfValidator.IsValid(cpf))
                return BadRequest(new {errors = new { cpf = new[] { "Valor inválido" } } });
            
            cpf = cpfValidator
                .Format(cpf, CodeFormat.Trim | CodeFormat.WithoutDigitSeparator | CodeFormat.WithoutNumberSeparator);

            Bill bill = new Bill()
            {
                DueDate = dataVencimento.Date,
                PersonId = cpf,
                Value = valorCobranca
            };

            bill = await _billService.CreateAsync(bill);

            return Ok(bill);
        }

    }
}
