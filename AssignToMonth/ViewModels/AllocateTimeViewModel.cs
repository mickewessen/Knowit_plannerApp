using AssignToMonth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace AssignToMonth.ViewModels
{
    public class AllocateTimeViewModel
    {
        public IEnumerable<AssignCustomerToMonth> GetAllCustomers { get; set; }
        public IPagedList<AllocateTime> AllocateTimes { get; set; }
        public IEnumerable<Month> Months { get; set; }
    }
}
