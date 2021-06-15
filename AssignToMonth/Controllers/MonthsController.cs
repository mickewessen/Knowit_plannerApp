using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssignToMonth.Data;
using AssignToMonth.Models;

namespace AssignToMonth.Controllers
{
    public class MonthsController : Controller
    {
        private readonly SqlDbContext _context;

        public MonthsController(SqlDbContext context)
        {
            _context = context;
        }

        // GET: Months
        public IActionResult Index(int? year)
        {
            ViewBag.Year = (from m in _context.Months
                            select m.Year).Distinct();

            var model = from m in _context.Months
                        orderby m.Year
                        where m.Year == year || year == null || year == 0
                        select m;
            return View(model);
        }

        #region CREATE METHOD
        // GET: Months/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Months/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MonthName,Year,Hours")] Month month)
        {
            if (ModelState.IsValid)
            {
                _context.Add(month);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(month);
        }

        #endregion

        #region EDIT METHOD
        // GET: Months/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var month = await _context.Months.FindAsync(id);
            if (month == null)
            {
                return NotFound();
            }
            return View(month);
        }

        // POST: Months/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MonthName,Year,Hours")] Month month)
        {
            if (id != month.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(month);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonthExists(month.Id))
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
            return View(month);
        }

        #endregion

        #region DELETE METHOD
        // GET: Months/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var month = await _context.Months
                .FirstOrDefaultAsync(m => m.Id == id);
            if (month == null)
            {
                return NotFound();
            }

            return View(month);
        }

        // POST: Months/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var month = await _context.Months.FindAsync(id);
            _context.Months.Remove(month);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion

        private bool MonthExists(int id)
        {
            return _context.Months.Any(e => e.Id == id);
        }
    }
}
