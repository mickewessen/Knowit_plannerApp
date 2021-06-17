using AssignToMonth.Data;
using AssignToMonth.Models;
using AssignToMonth.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace AssignToMonth.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly SqlDbContext _context;

        public DashboardController(ILogger<DashboardController> logger, SqlDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? year, string month, int? page, int currentYear, string currentMonth)
        {
            ViewBag.Year = (from item in _context.Months
                            select item.Year).Distinct();
            ViewBag.Month = (from item in _context.Months
                             select item.MonthName).Distinct();

            if (year != null || month != null)
            {
                page = 1;
            }
            else
            {
                year = currentYear;
                month = currentMonth;
            }

            ViewBag.CurrentYear = year;
            ViewBag.CurrentMonth = month;

            int pageSize = 12;
            int pageNumber = (page ?? 1);

            var dashBoard = new DashboardViewModel()
            {
                Users = _context.Users.ToList(),
                Customers = _context.Customers.ToList(),
                Months = _context.Months.ToList(),
                AssignedCustomerToMonths = (from item in _context.AssignCustomerToMonths
                                           orderby item.Month.Id ascending
                                           where item.Month.Year == year || year == null || year == 0
                                           where item.Month.MonthName == month || month == null || month == ""
                                           select item).ToPagedList(pageNumber, pageSize),

                AssignedUsersToMonths = (from item in _context.AssignedMonths.Include(m => m.AllocateTimes)
                                        orderby item.Month.Id ascending
                                        where item.Month.Year == year || year == null || year == 0
                                        where item.Month.MonthName == month || month == null || month == ""
                                        select item).ToPagedList(pageNumber, pageSize),            
            };                         
            return View(dashBoard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
