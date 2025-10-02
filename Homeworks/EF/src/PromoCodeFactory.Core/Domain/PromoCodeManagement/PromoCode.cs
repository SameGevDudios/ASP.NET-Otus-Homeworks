using PromoCodeFactory.Core.Domain.Administration;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    [Table("PromoCodes")]
    public class PromoCode
        : BaseEntity
    {
        [Required, MaxLength(36)]
        public string Code { get; set; }

        [MaxLength(100)]
        public string ServiceInfo { get; set; }

        [Required]
        public DateTime BeginDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required, MaxLength(30)]
        public string PartnerName { get; set; }

        public virtual Employee? PartnerManager { get; set; }

        public virtual Preference? Preference { get; set; }

        public virtual Customer? Customer { get; set; }
    }
}