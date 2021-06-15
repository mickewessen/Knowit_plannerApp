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

namespace AssignToMonth.Controllers
{
    public class AssignedUserToMonthsController : Controller
    {
        private readonly SqlDbContext _context;

        public AssignedUserToMonthsController(SqlDbContext context)
        {
            _context = context;
        }

        // GET: AssignedMonths
        public IActionResult Index(int? year, string month, string user, int? page, string currentUser, int currentYear, string currentMonth)
        {
            ViewBag.Year = (from item in _context.Months
                            select item.Year).Distinct();
            ViewBag.Month = (from item in _context.Months
                            select item.MonthName).Distinct();
            ViewBag.Users = (from item in _context.Users
                             select item.FirstName).Distinct();

            if (year != null || month != null || user != null )
            {
                page = 1;
            }
            else
            {
                year = currentYear;
                user = currentUser;
                month = currentMonth;                
            }

            ViewBag.CurrentUser = user;
            ViewBag.CurrentYear = year;
            ViewBag.CurrentMonth = month;

            var pageNumber = page ?? 1;
            int pageSize = 12;

            var model = from item in _context.AssignedMonths
                            .Include(a => a.Month)
                                .Include(a => a.User)
                        orderby item.Month.Id
                        where item.Month.Year == year || year == null || year == 0
                        where item.Month.MonthName == month || month == null || month ==""
                        where item.User.FirstName == user || user == null || user == ""
                        select item;

            return View(model.ToPagedList(pageNumber, pageSize));
        }

        #region CREATE METHOD

        // GET: AssignedMonths/Create
        public IActionResult Create()
        {
            ViewBag.Users = new SelectList(_context.Users, "Id", "DisplayName");
            ViewBag.Months = new SelectList(_context.Months, "Id", "DisplayMonth");

            List<AssignedMonth> assignedUsers = new List<AssignedMonth> { new AssignedMonth { Id = 0, MonthId = 0, UserId = 0, Absence = 0 } };
            return View(assignedUsers);
        }

        // POST: AssignedMonths/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<AssignedMonth> assignedUsers)
        {
            ViewBag.Users = new SelectList(_context.Users, "Id", "DisplayName");
            ViewBag.Months = new SelectList(_context.Months, "Id", "DisplayMonth");

            if (ModelState.IsValid)
            {
                foreach (var user in assignedUsers)
                {
                    _context.AssignedMonths.Add(user);
                    //if (!_context.AssignedMonths.Any(c => c.MonthId.Equals(user.MonthId)))
                    //{
                    //    _context.AssignedMonths.Add(user);
                    //    await _context.SaveChangesAsync();
                    //    ModelState.Clear();
                    //    return RedirectToAction(nameof(Index));
                    //}
                    //else
                    //{
                    //    ViewBag.Message = "En person är redan planerad på denna månad";
                    //}
                }
                await _context.SaveChangesAsync();
                ModelState.Clear();
                return RedirectToAction(nameof(Index));
            }
            return View(assignedUsers);
        }
        #endregion

        #region EDIT METHOD
        [HttpPost]
        public async Task<IActionResult> UpdateUser(AssignedMonth _user)
        {
            var updatedUser = await _context.AssignedMonths.Where(x => x.Id == _user.Id).SingleOrDefaultAsync();

            if (updatedUser != null)
            {
                updatedUser.Absence = _user.Absence;
                await _context.SaveChangesAsync();
            }
            return new EmptyResult();
        }
        #endregion

        #region DELETE METHOD
        // GET: AssignedMonths/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignedMonth = await _context.AssignedMonths
                .Include(a => a.Month)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignedMonth == null)
            {
                return NotFound();
            }

            return View(assignedMonth);
        }

        // POST: AssignedMonths/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignedMonth = await _context.AssignedMonths.FindAsync(id);
            _context.AssignedMonths.Remove(assignedMonth);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssignedMonthExists(int id)
        {
            return _context.AssignedMonths.Any(e => e.Id == id);
        }
    }
    #endregion
}
