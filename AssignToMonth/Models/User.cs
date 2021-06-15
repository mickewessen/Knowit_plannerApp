using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AssignToMonth.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Fyll i Förnamn")]
        [Column(TypeName = "nvarchar(25)")]
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Fyll i Efternamn")]
        [Column(TypeName = "nvarchar(25)")]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Display(Name = "Namn")]
        public string DisplayName => $"{FirstName} {LastName}";

        public virtual ICollection<AssignedMonth> AssignedMonths { get; set; }
    }
}
