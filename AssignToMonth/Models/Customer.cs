using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AssignToMonth.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Fyll i en kund")]
        [Column(TypeName = "nvarchar(50)")]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        public virtual ICollection<AssignCustomerToMonth> AssignCustomerToMonths { get; set; }
    }
}
