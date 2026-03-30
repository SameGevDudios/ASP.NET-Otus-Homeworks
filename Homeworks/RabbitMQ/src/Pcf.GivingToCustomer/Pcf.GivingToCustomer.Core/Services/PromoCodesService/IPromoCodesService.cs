using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Services.PromoCodesService
{
    public interface IPromoCodesService
    {
        Task GivePromoCodeToCustomersWithPreferenceAsync(GivePromoCodeDto dto);
    }
}
