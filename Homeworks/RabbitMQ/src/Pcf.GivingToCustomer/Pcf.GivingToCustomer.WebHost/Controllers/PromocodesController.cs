using Microsoft.AspNetCore.Mvc;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.Core.Services.PromoCodesService;
using Pcf.GivingToCustomer.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController
        : ControllerBase
    {
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IPromoCodesService _promoCodesService;

        public PromocodesController(IRepository<PromoCode> promoCodesRepository, IPromoCodesService promoCodesService)
        {
            _promoCodesRepository = promoCodesRepository;
            _promoCodesService = promoCodesService;
        }

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var promocodes = await _promoCodesRepository.GetAllAsync();

            var response = promocodes.Select(x => new PromoCodeShortResponse()
            {
                Id = x.Id,
                Code = x.Code,
                BeginDate = x.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = x.EndDate.ToString("yyyy-MM-dd"),
                PartnerId = x.PartnerId,
                ServiceInfo = x.ServiceInfo
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            var dto = new GivePromoCodeDto
            {
                PromoCodeId = request.PromoCodeId,
                PartnerId = request.PartnerId,
                PromoCode = request.PromoCode,
                ServiceInfo = request.ServiceInfo,
                PreferenceId = request.PreferenceId,
                BeginDate = DateTime.Parse(request.BeginDate),
                EndDate = DateTime.Parse(request.EndDate)
            };

            await _promoCodesService.GivePromoCodeToCustomersWithPreferenceAsync(dto);

            return CreatedAtAction(nameof(GetPromocodesAsync), new { }, null);
        }
    }
}