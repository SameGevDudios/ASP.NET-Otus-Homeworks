using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    [Table("Preference")]
    public class Preference
        : BaseEntity
    {
        [Required, MaxLength(30)]
        public string Name { get; set; }

        public ICollection<CustomerPreference> CustomerPreferences { get; set; }
    }
}