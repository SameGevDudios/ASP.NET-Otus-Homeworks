using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    [Table("CustomerPreferences")]
    public class CustomerPreference : BaseEntity
    {
        public Guid? CustomerId { get; set; }

        public Customer? Customer { get; set; }

        public Guid? PreferenceId { get; set; }

        public Preference? Preference { get; set; }
    }
}
