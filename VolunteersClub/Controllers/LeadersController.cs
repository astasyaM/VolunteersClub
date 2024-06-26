﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VolunteersClub.Data;
using VolunteersClub.Models;

namespace VolunteersClub.Controllers
{
    public class LeadersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LeadersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Leaders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Leaders.ToListAsync());
        }

        // GET: Leaders/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leader = await _context.Leaders
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (leader == null)
            {
                return NotFound();
            }

            return View(leader);
        }

        // GET: Leaders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Leaders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistrationLeader model)
        {
            if (model.BirthDate.Year < 1924 || model.BirthDate.Year > 2009)
            {
                ModelState.AddModelError("BirthDate", "Введите корректную дату рождения");
            }
            if (model.Confirm!="БольшоеДоброеДело")
            {
                ModelState.AddModelError("Confirm", "Неправильный ключ подтверждения");
            }
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                Leader leader = new Leader
                {
                    UserID = user.Id,
                    Name = model.Name,
                    Patronymic = model.Patronymic,
                    Surname = model.Surname,
                    BirthDate = model.BirthDate
                };

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Leader");
                    _context.Add(leader);
                    await _context.SaveChangesAsync();
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Переадресация на личный кабинет
                    return RedirectToAction("Details", "Leaders", new { id = leader.UserID });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // GET: Leaders/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leader = await _context.Leaders
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (leader == null)
            {
                return NotFound();
            }
            return View(leader);
        }

        // POST: Leaders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LeaderID,UserID,Name,Surname,Patronymic,BirthDate")] Leader leader)
        {
            if (id != leader.LeaderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leader);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaderExists(leader.LeaderID))
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
            return View(leader);
        }

        // GET: Leaders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leader = await _context.Leaders
                .FirstOrDefaultAsync(m => m.LeaderID == id);
            if (leader == null)
            {
                return NotFound();
            }

            return View(leader);
        }

        // POST: Leaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leader = await _context.Leaders.FindAsync(id);
            if (leader != null)
            {
                _context.Leaders.Remove(leader);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaderExists(int id)
        {
            return _context.Leaders.Any(e => e.LeaderID == id);
        }
    }
}
