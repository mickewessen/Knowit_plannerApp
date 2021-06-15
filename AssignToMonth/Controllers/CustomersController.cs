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
    public class CustomersController : Controller
    {
        private readonly SqlDbContext _context;

        public CustomersController(SqlDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.ToListAsync());
        }

        #region CREATE METHOD

        // GET: Customers/Create
        public IActionResult Create()
        {
            List<Customer> customers = new List<Customer> { new Customer { Id = 0, Name = "" } };
            return View(customers);
        }


        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<Customer> customers)
        {
            if (ModelState.IsValid)
            {
                foreach (var customer in customers)
                {
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();

                    foreach (var month in _context.Months)
                    {
                        AssignCustomerToMonth model = new AssignCustomerToMonth();
                        model.CustomerId = customer.Id;
                        for (int i = 0; i < month.Id; i++)
                        {
                            model.MonthId = month.Id;
                        }
                        model.FTE = 0;
                        _context.AssignCustomerToMonths.Add(model);
                    }
                }
                await _context.SaveChangesAsync();
                ModelState.Clear();
                return RedirectToAction(nameof(Index));
            }
            return View(customers);
        }

        #endregion

        #region EDIT METHOD
        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
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
            return View(customer);
        }

        #endregion

        #region DELETE METHOD

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }

        #endregion
    }
}
