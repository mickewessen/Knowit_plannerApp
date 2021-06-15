using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AssignToMonth.Models
{
    public class Month
    {
        public int Id { get; set; }
        [Display(Name = "Månad")]
        public string MonthName { get; set; }
        [Display(Name = "År")]
        public int Year { get; set; }
        [Display(Name = "Timmar")]
        public int Hours { get; set; }

        public virtual ICollection<AssignedMonth> AssignedMonths { get; set; }
        public virtual ICollection<AssignCustomerToMonth> AssignCustomerToMonths { get; set; }

        [Display(Name = "Månad")]
        public string DisplayMonth => $"{MonthName} {Year}";
    }
}
