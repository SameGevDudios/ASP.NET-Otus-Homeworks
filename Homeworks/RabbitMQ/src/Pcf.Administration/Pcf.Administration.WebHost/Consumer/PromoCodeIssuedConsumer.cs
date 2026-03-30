using MassTransit;
using Pcf.Administration.Core.Services;
using Pcf.Administration.WebHost.Messages;
using System.Threading.Tasks;

namespace Pcf.Administration.WebHost.Consumer
{
    public class PromoCodeIssuedConsumer : IConsumer<PromoCodeIssuedEvent>
    {
        private readonly IEmployeeService _employeeService;

        public PromoCodeIssuedConsumer(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task Consume(ConsumeContext<PromoCodeIssuedEvent> context)
        {
            if (context.Message.PartnerManagerId.HasValue)
            {
                await _employeeService.UpdateAppliedPromocodesAsync(context.Message.PartnerManagerId.Value);
            }
        }
    }
}
