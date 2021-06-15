using AssignToMonth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignToMonth.ViewModels
{
    public class DashboardViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Customer> Customers { get; set; }
        public IEnumerable<AssignCustomerToMonth> AssignedCustomerToMonths { get; set; }
        public IEnumerable<AssignedMonth> AssignedUsersToMonths { get; set; }
        public IEnumerable<Month> Months { get; set; }
        public IEnumerable<AllocateTime> AllocateTimes { get; set; }

        public int TotalUsers { get { return Users.Count(); } }
        public int TotalProjects { get { return Customers.Count(); } }

    }
}
