using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    [Table("CustomerPreferences")]
    public class CustomerPreference : BaseEntity
    {
        [Required]
        public Guid CustomerId { get; set; }
        [Required]
        public Customer Customer { get; set; }

        [Required]
        public Guid PreferenceId { get; set; }

        [Required]
        public Preference Preference { get; set; }
    }
}
