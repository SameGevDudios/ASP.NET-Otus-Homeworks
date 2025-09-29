using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    [Table("Customers")]
    public class Customer
        : BaseEntity
    {
        [Required, MaxLength(30)]
        public string FirstName { get; set; }

        [Required, MaxLength(30)]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [Required, MaxLength(30)]
        public string Email { get; set; }

        //TODO: Списки Preferences и Promocodes 
        public ICollection<CustomerPreference> CustomerPreferences { get; set; }

        public ICollection<PromoCode> Promocodes { get; set; }
    }
}