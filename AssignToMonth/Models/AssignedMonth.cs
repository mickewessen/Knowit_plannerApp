using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AssignToMonth.Models
{
    public class AssignedMonth
    {
        public int Id { get; set; }
        [Display(Name = "Användare")]
        public int UserId { get; set; }

        [Display(Name = "Månad")]
        public int MonthId { get; set; }

        [Display(Name = "Frånvaro")]
        public int Absence { get; set; }

        [Display(Name = "Användare")]
        public virtual User User { get; set; }

        [Display(Name = "Månad")]
        public virtual Month Month { get; set; }

        public ICollection<AllocateTime> AllocateTimes { get; set; }

        public double TotalHours() {
            var x = Month.Hours;
            var y = Absence;
            var z = 0.95;
            var result = Math.Round((x*z) - y);

            return result;
        }

    }
}
