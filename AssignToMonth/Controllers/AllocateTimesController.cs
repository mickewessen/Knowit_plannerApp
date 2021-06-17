using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssignToMonth.Data;
using AssignToMonth.Models;
using AssignToMonth.ViewModels;
using X.PagedList;

namespace AssignToMonth.Controllers
{
    public class AllocateTimesController : Controller
    {
        private readonly SqlDbContext _context;

        public AllocateTimesController(SqlDbContext context)
        {
            _context = context;
        }

        // GET: AllocateTimes
        public IActionResult Index(int? year, string month, string customer, int? page, int currentYear, string currentMonth, string currentCustomer)
        {
            ViewBag.Year = (from item in _context.Months
                            orderby item.Year
                            select item.Year).Distinct();
            ViewBag.Month = (from item in _context.Months
                             orderby item.Id
                             select item.MonthName).Distinct();
            ViewBag.Customers = (from item in _context.Customers
                                 orderby item.Id
                                 select item.Name).Distinct();

            if (year != null || month != null || customer != null)
            {
                page = 1;
            }
            else
            {
                year = currentYear;
                customer = currentCustomer;
                month = currentMonth;
            }

            ViewBag.CurrentCustomer = customer;
            ViewBag.CurrentYear = year;
            ViewBag.CurrentMonth = month;

            int pageNumber = page ?? 1;
            int pageSize = 12;

            var viewModel = new AllocateTimeViewModel()
            {
                GetAllCustomers = from item in _context.AssignCustomerToMonths.Include(c => c.Customer)
                                            .Include(m => m.Month)
                                                .Include(a => a.AllocateTimes)
                                  orderby item.Month.Id
                                  where item.Month.Year == year || year == null || year == 0
                                  where item.Month.MonthName == month || month == null || month == ""
                                  where item.Customer.Name == customer || customer == null || customer == ""
                                  select item,

                AllocateTimes = (from item in _context.AllocateTime
                                       .Include(u => u.User.User)
                                         .Include(u => u.Customer.Customer)
                                             .Include(u => u.User.Month)
                                 orderby item.User.Month.Id
                                 where item.User.Month.Year == year || year == null || year == 0
                                 where item.User.Month.MonthName == month || month == null || month == ""
                                 where item.Customer.Customer.Name == customer || customer == null || customer == ""
                                 select item).ToPagedList(pageNumber, pageSize),   
            };

            return View(viewModel);
        }

        #region CREATE METHOD
        // GET: AllocateTimes/Create
        public IActionResult Create()
        {
            ViewBag.Customer = new SelectList(_context.AssignCustomerToMonths, "Id", "CustomerId");
            ViewBag.User = new SelectList(_context.AssignedMonths, "Id", "UserId");
            return View();
        }

        // POST: AllocateTimes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AllocateTime allocateTime)
        {
            ViewBag.Customer = new SelectList(_context.AssignCustomerToMonths, "Id", "CustomerId");
            ViewBag.User = new SelectList(_context.AssignedMonths, "Id", "UserId");

            if (ModelState.IsValid)
            {
                _context.Add(allocateTime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(allocateTime);
        }

        #endregion

        #region EDIT METHOD
        // GET: AllocateTimes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var allocateTime = await _context.AllocateTime.FindAsync(id);
            if (allocateTime == null)
            {
                return NotFound();
            }
            return View(allocateTime);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(AllocateTime _user)
        {
            var months = await _context.Months.ToListAsync();

            //Hämta användarens totala arbetstid som ska planeras ut per månad
            var getUser = await _context.AssignedMonths.FindAsync(_user.UserId);
            var userTotalHours = getUser.TotalHours();

            //Hämta användarens aktuella utplanerade tid per månad
            var getUserTime = await _context.AllocateTime.FindAsync(_user.Id);
            var userTime = getUserTime.AllocatedHours;

            //Hämta användarens tid utplanerad på alla kunder per månad
            var getUserSumHours = _context.AllocateTime.Where(u => u.UserId == _user.UserId).Select(x => x.AllocatedHours);
            var userSumHours = getUserSumHours.Sum();


            //Hämta kundens totala FTE som ska planeras ut per månad
            var getCustomer = await _context.AssignCustomerToMonths.FindAsync(_user.CustomerId);
            var customerTotalHours = getCustomer.FteToHours();

            //Hämta kundens aktuella utplanerade tid per månad
            var getCustomerTime = await _context.AllocateTime.FindAsync(_user.Id);
            var customerTime = getCustomerTime.AllocatedHours;

            //Hämta kundens tid utplanerad på alla användare per månad
            var getCustomerAllHours = _context.AllocateTime.Where(u => u.CustomerId == _user.CustomerId).Select(x => x.AllocatedHours);
            var customerSumHours = getCustomerAllHours.Sum();

            //Beräkning av nuvarande planerad tid mot ny planerad tid
            var userTimeDiff = userSumHours - userTime + _user.AllocatedHours;
            var customerTimeDiff = customerSumHours - customerTime + _user.AllocatedHours;

            var updatedUser = await _context.AllocateTime.Where(x => x.Id == _user.Id).SingleOrDefaultAsync();

            if (updatedUser != null)
            {
                if (userTimeDiff <= userTotalHours && customerTimeDiff <= customerTotalHours)
                {
                    updatedUser.AllocatedHours = _user.AllocatedHours;
                    await _context.SaveChangesAsync();
                }
                else if (userTimeDiff > userTotalHours)
                {
                    TempData["UserError"] = "Du kan inte planera ut mer tid på denna användare";
                }
                else if (customerTimeDiff > customerTotalHours)
                {
                    TempData["CustomerError"] = "Du kan inte planera ut mer tid på denna kund";
                }
            }
            return new EmptyResult();
        }
        #endregion

        #region DELETE METHOD
        // GET: AllocateTimes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var allocateTime = await _context.AllocateTime
                .FirstOrDefaultAsync(m => m.Id == id);
            if (allocateTime == null)
            {
                return NotFound();
            }

            return View(allocateTime);
        }

        // POST: AllocateTimes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var allocateTime = await _context.AllocateTime.FindAsync(id);
            _context.AllocateTime.Remove(allocateTime);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion

        private bool AllocateTimeExists(int id)
        {
            return _context.AllocateTime.Any(e => e.Id == id);
        }
                
    }
}
