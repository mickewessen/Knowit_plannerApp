using AssignToMonth.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignToMonth.Data
{
    public class SqlDbContext:DbContext
    {
        public SqlDbContext()
        {
        }

        public SqlDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Month> Months { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<AssignedMonth> AssignedMonths { get; set; }
        public DbSet<AssignCustomerToMonth> AssignCustomerToMonths { get; set; }
        public DbSet<AllocateTime> AllocateTime { get; set; }
    }
}
