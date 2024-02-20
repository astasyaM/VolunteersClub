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
    public class VolunteerStatusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VolunteerStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: VolunteerStatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.VolunteerStatuses.ToListAsync());
        }

        // GET: VolunteerStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteerStatus = await _context.VolunteerStatuses
                .FirstOrDefaultAsync(m => m.StatusID == id);
            if (volunteerStatus == null)
            {
                return NotFound();
            }

            return View(volunteerStatus);
        }

        // GET: VolunteerStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VolunteerStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VolunteerStatusID,VolunteerStatusName")] VolunteerStatus volunteerStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(volunteerStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(volunteerStatus);
        }

        // GET: VolunteerStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteerStatus = await _context.VolunteerStatuses.FindAsync(id);
            if (volunteerStatus == null)
            {
                return NotFound();
            }
            return View(volunteerStatus);
        }

        // POST: VolunteerStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VolunteerStatusID,VolunteerStatusName")] VolunteerStatus volunteerStatus)
        {
            if (id != volunteerStatus.StatusID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(volunteerStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VolunteerStatusExists(volunteerStatus.StatusID))
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
            return View(volunteerStatus);
        }

        // GET: VolunteerStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteerStatus = await _context.VolunteerStatuses
                .FirstOrDefaultAsync(m => m.StatusID == id);
            if (volunteerStatus == null)
            {
                return NotFound();
            }

            return View(volunteerStatus);
        }

        // POST: VolunteerStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var volunteerStatus = await _context.VolunteerStatuses.FindAsync(id);
            if (volunteerStatus != null)
            {
                _context.VolunteerStatuses.Remove(volunteerStatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VolunteerStatusExists(int id)
        {
            return _context.VolunteerStatuses.Any(e => e.StatusID == id);
        }
    }
}
