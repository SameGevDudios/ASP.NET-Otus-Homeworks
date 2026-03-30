using MassTransit;
using Pcf.GivingToCustomer.Core.Services.PromoCodesService;
using Pcf.GivingToCustomer.WebHost.Messages;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.Consumers
{
    public class PromoCodeIssuedConsumer : IConsumer<PromoCodeIssuedEvent>
    {
        private readonly IPromoCodesService _promoCodesService;

        public PromoCodeIssuedConsumer(IPromoCodesService promoCodesService)
        {
            _promoCodesService = promoCodesService;
        }

        public async Task Consume(ConsumeContext<PromoCodeIssuedEvent> context)
        {
            var message = context.Message;
            var dto = new GivePromoCodeDto
            {
                PromoCodeId = message.PromoCodeId,
                PartnerId = message.PartnerId,
                PromoCode = message.PromoCode,
                ServiceInfo = message.ServiceInfo,
                PreferenceId = message.PreferenceId,
                BeginDate = message.BeginDate,
                EndDate = message.EndDate
            };

            await _promoCodesService.GivePromoCodeToCustomersWithPreferenceAsync(dto);
        }
    }
}
