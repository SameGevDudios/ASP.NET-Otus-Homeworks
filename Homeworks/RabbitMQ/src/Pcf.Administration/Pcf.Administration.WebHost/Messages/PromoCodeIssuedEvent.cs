using System;

namespace Pcf.Administration.WebHost.Messages
{
    public class PromoCodeIssuedEvent
    {
        public Guid PromoCodeId { get; set; }
        public Guid PartnerId { get; set; }
        public string PromoCode { get; set; }
        public string ServiceInfo { get; set; }
        public Guid PreferenceId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? PartnerManagerId { get; set; }
    }
}
