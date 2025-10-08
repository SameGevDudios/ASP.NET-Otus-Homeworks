using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController
        : ControllerBase
    {

        private readonly IRepository<PromoCode> _promoCodeRepository;
        private readonly IRepository<CustomerPreference> _customerPreferenceRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly ICodeGenerator _codeGenerator;

        public PromocodesController(
            IRepository<PromoCode> promoCodeRepository,
            IRepository<CustomerPreference> customerPreferenceRepository,
            IRepository<Customer> customerRepository,
            ICodeGenerator codeGenerator)
        {
            _promoCodeRepository = promoCodeRepository;
            _customerPreferenceRepository = customerPreferenceRepository;
            _customerRepository = customerRepository;
            _codeGenerator = codeGenerator;
        }

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var promoCodes = await _promoCodeRepository.GetAllAsync();
            return promoCodes.Select(promoCode =>
                new PromoCodeShortResponse()
                {
                    Id = promoCode.Id,
                    Code = promoCode.Code,
                    BeginDate = promoCode.BeginDate.ToString(),
                    EndDate = promoCode.EndDate.ToString(),
                    PartnerName = promoCode.PartnerName,
                    ServiceInfo = promoCode.ServiceInfo
                }).ToList();
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            var customers = (await _customerPreferenceRepository.GetAllAsync()) // Get all customer preferences
                .Where(cp => cp.Preference.Name == request.Preference) // Get requested preference entries
                .Select(cp => cp.Customer); // Get customers
            List<PromoCodeShortResponse> promoCodeResponseList = new();
            Console.WriteLine($"customers: {customers.Count()}");
            foreach (var customer in customers)
            {
                PromoCode promoCode = new PromoCode()
                {
                    Id = Guid.NewGuid(),
                    BeginDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(14),
                    Code = String.IsNullOrEmpty(request.PromoCode) ? _codeGenerator.Code() : request.PromoCode,
                    Customer = customer,
                    ServiceInfo = request.ServiceInfo,
                    PartnerName = request.PartnerName,
                    PartnerManager = null // todo
                };

                customer.Promocodes.Add(promoCode);
                await _promoCodeRepository.AddAsync(promoCode);
                await _customerRepository.UpdateAsync(customer);

                promoCodeResponseList.Add(
                    new PromoCodeShortResponse()
                    {
                        Id = promoCode.Id,
                        Code = promoCode.Code,
                        BeginDate = promoCode.BeginDate.ToString(),
                        EndDate = promoCode.EndDate.ToString(),
                        PartnerName = promoCode.PartnerName,
                        ServiceInfo = promoCode.ServiceInfo,
                    });
            }

            return CreatedAtAction("GetPromocodes", null, promoCodeResponseList);
        }
    }
}