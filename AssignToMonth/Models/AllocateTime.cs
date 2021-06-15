using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignToMonth.Models
{
    public class AllocateTime
    {
        public int Id { get; set; }
        public int AllocatedHours { get; set; }
        public int UserId { get; set; }
        public int CustomerId { get; set; }

        public virtual AssignCustomerToMonth Customer { get; set; }
        public virtual AssignedMonth User { get; set; }

        public decimal TotalTime ()
        {
            var x = Customer.FTE;
            var u = AllocatedHours;
            var result = Math.Round((x * 160) - u);
            return result;
        }

    }
}
