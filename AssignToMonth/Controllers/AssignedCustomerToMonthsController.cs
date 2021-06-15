using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssignToMonth.Data;
using AssignToMonth.Models;
using X.PagedList;
using Newtonsoft.Json;

namespace AssignToMonth.Controllers
{
    public class AssignedCustomerToMonthsController : Controller
    {
        private readonly SqlDbContext _context;

        public AssignedCustomerToMonthsController(SqlDbContext context)
        {
            _context = context;
        }

        // GET: AssignCustomerToMonths
        public IActionResult Index(int? year, string month, int? page, int currentYear, string currentMonth)
        {
            ViewBag.Year = (from item in _context.Months
                            orderby item.Year
                            select item.Year).Distinct();
            ViewBag.Month = (from item in _context.Months
                             orderby item.Id
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

            var pageNumber = page ?? 1;
            int pageSize = 12;

            var model = from item in _context.AssignCustomerToMonths
                            .Include(a => a.Customer)
                                .Include(a => a.Month)
                        orderby item.Month.Id   
                        where item.Month.Year == year || year == null || year == 0
                        where item.Month.MonthName == month || month == null || month ==""
                        select item;

            return View(model.ToPagedList(pageNumber, pageSize));
        }

        #region CREATE METHOD
        // GET: AssignCustomerToMonths/Create
        public IActionResult Create()
        {
            ViewBag.Customers = new SelectList(_context.Customers, "Id", "Name");
            ViewBag.Months = new SelectList(_context.Months, "Id", "DisplayMonth");

            List<AssignCustomerToMonth> assignedCustomers = new List<AssignCustomerToMonth> { new AssignCustomerToMonth { Id = 0, CustomerId = 0, MonthId = 0, FTE = 0} };
            return View(assignedCustomers);
        }

        // POST: AssignCustomerToMonths/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<AssignCustomerToMonth> assignCustomers)
        {
            ViewBag.Customers = new SelectList(_context.Customers, "Id", "Name");
            ViewBag.Months = new SelectList(_context.Months, "Id", "DisplayMonth");

            if (ModelState.IsValid)
            {
                foreach (var customer in assignCustomers)
                {
                    _context.Add(customer);
                }

                await _context.SaveChangesAsync();
                ModelState.Clear();
                return RedirectToAction(nameof(Index));
            }
            return View(assignCustomers);
        }


        //// POST: AssignCustomerToMonths/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,CustomerId,MonthId,FTE")] AssignCustomerToMonth assignCustomerToMonth)
        //{
        //    ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", assignCustomerToMonth.CustomerId);
        //    ViewData["MonthId"] = new SelectList(_context.Months, "Id", "DisplayMonth", assignCustomerToMonth.MonthId);

        //    if (ModelState.IsValid)
        //    {
        //        if (!_context.AssignCustomerToMonths.Any(m => m.MonthId.Equals(assignCustomerToMonth.MonthId)))
        //        {
        //            _context.Add(assignCustomerToMonth);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        else
        //        {
        //            ViewBag.Message = "Kunden är redan planerad för denna månad";
        //        }

        //    }
        //    return View(assignCustomerToMonth);
        //}

        #endregion

        #region EDIT METHOD
        // GET: AssignCustomerToMonths/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignCustomerToMonth = await _context.AssignCustomerToMonths.FindAsync(id);
            if (assignCustomerToMonth == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", assignCustomerToMonth.CustomerId);
            ViewData["MonthId"] = new SelectList(_context.Months, "Id", "DisplayMonth", assignCustomerToMonth.MonthId);
            return View(assignCustomerToMonth);
        }

        // POST: AssignCustomerToMonths/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,MonthId,FTE")] AssignCustomerToMonth assignCustomerToMonth)
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", assignCustomerToMonth.CustomerId);
            ViewData["MonthId"] = new SelectList(_context.Months, "Id", "DisplayMonth", assignCustomerToMonth.MonthId);

            if (id != assignCustomerToMonth.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignCustomerToMonth);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignCustomerToMonthExists(assignCustomerToMonth.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(assignCustomerToMonth);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(AssignCustomerToMonth _customer)
        {

            var updatedCustomer = await _context.AssignCustomerToMonths.Where(x => x.Id == _customer.Id).SingleOrDefaultAsync();

            if (updatedCustomer != null)
            {
                updatedCustomer.FTE = _customer.FTE;
                await _context.SaveChangesAsync();
            }
            return new EmptyResult();
        }

        #endregion

        #region DELETE METHOD
        // GET: AssignCustomerToMonths/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignCustomerToMonth = await _context.AssignCustomerToMonths
                .Include(a => a.Customer)
                    .Include(a => a.Month)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignCustomerToMonth == null)
            {
                return NotFound();
            }

            return View(assignCustomerToMonth);
        }

        // POST: AssignCustomerToMonths/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignCustomerToMonth = await _context.AssignCustomerToMonths.FindAsync(id);
            _context.AssignCustomerToMonths.Remove(assignCustomerToMonth);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssignCustomerToMonthExists(int id)
        {
            return _context.AssignCustomerToMonths.Any(e => e.Id == id);
        }

        #endregion

    }
}
