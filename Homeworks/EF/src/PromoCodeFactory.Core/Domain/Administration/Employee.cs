using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromoCodeFactory.Core.Domain.Administration
{
    [Table("Employees")]
    public class Employee
        : BaseEntity
    {
        [Required, MaxLength(30)]
        public string FirstName { get; set; }

        [Required, MaxLength(30)]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [Required, MaxLength(30)]
        public string Email { get; set; }

        [Required]
        public virtual Role Role { get; set; }

        public virtual ICollection<PromoCode> PromoCodes { get; set; } = new List<PromoCode>();

        public int AppliedPromocodesCount { get; set; }
    }
}