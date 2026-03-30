using System;

namespace Pcf.GivingToCustomer.Core.Services.PromoCodesService
{
    public class GivePromoCodeDto
    {
        public Guid PromoCodeId { get; set; }
        public Guid PartnerId { get; set; }
        public string PromoCode { get; set; }
        public string ServiceInfo { get; set; }
        public Guid PreferenceId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
