using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Services.PromoCodesService
{
    public class PromoCodesService : IPromoCodesService
    {
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IRepository<Preference> _preferencesRepository;
        private readonly IRepository<Customer> _customersRepository;

        public PromoCodesService(
            IRepository<PromoCode> promoCodesRepository,
            IRepository<Preference> preferencesRepository,
            IRepository<Customer> customersRepository)
        {
            _promoCodesRepository = promoCodesRepository;
            _preferencesRepository = preferencesRepository;
            _customersRepository = customersRepository;
        }

        public async Task GivePromoCodeToCustomersWithPreferenceAsync(GivePromoCodeDto dto)
        {
            var preference = await _preferencesRepository.GetByIdAsync(dto.PreferenceId);
            if (preference == null) return;

            var customers = await _customersRepository.GetWhere(d => d.Preferences.Any(x => x.PreferenceId == preference.Id));

            var promoCode = new PromoCode
            {
                Id = dto.PromoCodeId,
                PartnerId = dto.PartnerId,
                Code = dto.PromoCode,
                ServiceInfo = dto.ServiceInfo,
                BeginDate = dto.BeginDate,
                EndDate = dto.EndDate,
                PreferenceId = preference.Id,
                Preference = preference,
                Customers = customers.Select(c => new PromoCodeCustomer { CustomerId = c.Id }).ToList()
            };

            await _promoCodesRepository.AddAsync(promoCode);
        }
    }
}
