using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VolunteersClub.Data;
using VolunteersClub.Models;

namespace VolunteersClub.Controllers
{
    public class ResponsibilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResponsibilitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Responsibilities
        public async Task<IActionResult> Index()
        {
            return View(await _context.Responsibilities.ToListAsync());
        }

        // GET: Responsibilities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var responsibility = await _context.Responsibilities
                .FirstOrDefaultAsync(m => m.ResponsibilityID == id);
            if (responsibility == null)
            {
                return NotFound();
            }

            return View(responsibility);
        }

        // GET: Responsibilities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Responsibilities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ResponsibilityID,ResponsibilityName")] Responsibility responsibility)
        {
            if (ModelState.IsValid)
            {
                _context.Add(responsibility);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(responsibility);
        }

        // GET: Responsibilities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var responsibility = await _context.Responsibilities.FindAsync(id);
            if (responsibility == null)
            {
                return NotFound();
            }
            return View(responsibility);
        }

        // POST: Responsibilities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ResponsibilityID,ResponsibilityName")] Responsibility responsibility)
        {
            if (id != responsibility.ResponsibilityID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(responsibility);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResponsibilityExists(responsibility.ResponsibilityID))
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
            return View(responsibility);
        }

        // GET: Responsibilities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var responsibility = await _context.Responsibilities
                .FirstOrDefaultAsync(m => m.ResponsibilityID == id);
            if (responsibility == null)
            {
                return NotFound();
            }

            return View(responsibility);
        }

        // POST: Responsibilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var responsibility = await _context.Responsibilities.FindAsync(id);
            if (responsibility != null)
            {
                _context.Responsibilities.Remove(responsibility);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResponsibilityExists(int id)
        {
            return _context.Responsibilities.Any(e => e.ResponsibilityID == id);
        }
    }
}
