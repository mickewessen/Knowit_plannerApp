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
    public class UsersController : Controller
    {
        private readonly SqlDbContext _context;

        public UsersController(SqlDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public IActionResult Index()
        {
            var usersToEdit = from item in _context.Users
                              orderby item.Id
                              select item;         

            return View(usersToEdit.ToList());            
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            List<User> user = new List<User> { new User { Id = 0, FirstName = "", LastName = "" } };
            return View(user);
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<User> users)
        {
            if (ModelState.IsValid)
            {
                foreach (var user in users)
                {
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    foreach (var month in _context.Months)
                    {
                        AssignedMonth model = new AssignedMonth();
                        model.UserId = user.Id;
                        for (int i = 0; i < month.Id; i++)
                        {
                            model.MonthId = month.Id;
                        }
                        model.Absence = 0;
                        _context.AssignedMonths.Add(model);
                    }
                }
                await _context.SaveChangesAsync();
                ModelState.Clear();
                return RedirectToAction(nameof(Index));
            }
            return View(users);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateUser(User user)
        {
            User updatedUser = (from u in _context.Users
                                where u.Id == user.Id
                                select u).FirstOrDefault();

            updatedUser.FirstName = user.FirstName;
            updatedUser.LastName = user.LastName;
            await _context.SaveChangesAsync();

            return new EmptyResult();
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
