using Company.BillingSystem.Common.Validators;
using Company.BillingSystem.CustomerApi.Models;
using Company.BillingSystem.CustomerApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.BillingSystem.CustomerApi.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private CpfValidator _cpfValidator;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
            _cpfValidator = new CpfValidator();
        }

        [HttpGet("{cpf}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Customer))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Customer>> GetById(string cpf)
        {
            if (_cpfValidator.IsValid(cpf))
            {
                Customer customer = await _customerService.GetAsync(cpf);
                if (customer == null)
                    return NotFound();
                
                return Ok(customer);
            }
            else
                return BadRequest(new { errors = new { id = new[] { "CPF inválido" } } });
        }

        /// <summary>
        /// It is an extra method created to get all person ids
        /// </summary>
        /// <returns>List of person ids (cpf)</returns>

        [HttpGet(@"cpfs")]
        public async IAsyncEnumerable<string> GetIdList()
        {
            await foreach (string id in _customerService.GetIdListAsync())
            {
                yield return id;
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Customer))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Customer>> Create(string nome, string estado, string cpf)
        {
            List<string> errorMessages = new List<string>();

            if (String.IsNullOrEmpty(nome))
                errorMessages.Add("'Nome' não pode estar em branco");
            if (String.IsNullOrEmpty(estado))
                errorMessages.Add("'Estado' não pode estar em branco");
            if (String.IsNullOrEmpty(cpf))
                errorMessages.Add("'CPF' não pode estar em branco");

            if (!_cpfValidator.IsValid(cpf))
                errorMessages.Add("CPF inválido");

            if(errorMessages.Count > 0)
            {
                return BadRequest(new { errors = errorMessages });
            }

            cpf = _cpfValidator.Format(cpf, CodeFormat.Trim | CodeFormat.WithoutDigitSeparator | CodeFormat.WithoutNumberSeparator);

            Customer customer = await _customerService.GetAsync(cpf);

            if(customer != null)
                return BadRequest(new { error = "Não é permitido CPF duplicado" });

            customer = new Customer()
            {
                Cpf = cpf,
                Name = nome,
                State = estado
            };

            await _customerService.CreateAsync(customer);

            return CreatedAtAction(nameof(GetById), new { cpf = cpf }, customer);
        }


    }
}
