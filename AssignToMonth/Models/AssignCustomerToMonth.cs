using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AssignToMonth.Models
{
    public class AssignCustomerToMonth
    {
        public int Id { get; set; }
        [Display(Name = "Kund")]
        public int CustomerId { get; set; }
        [Display(Name = "Månad")]
        public int MonthId { get; set; }
        public int FTE { get; set; }

        [Display(Name = "Kund")]
        public virtual Customer Customer { get; set; }

        [Display(Name = "Månad")]
        public virtual Month Month { get; set; }

        public ICollection<AllocateTime> AllocateTimes { get; set; }

        public int FteToHours()
        {
            int x = 160;
            int y = FTE;
            var result = x * y;

            return result;
        }
    }
}
