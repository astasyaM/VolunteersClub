using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using VolunteersClub.Data;
using VolunteersClub.Models;

namespace VolunteersClub.Controllers
{
    public class VolunteersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VolunteersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Volunteers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Volunteers.ToListAsync());
        }

        // GET: Volunteers/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _context.Volunteers
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // GET: Volunteers/Create
        public IActionResult Create()
        {
            ViewBag.EventTypes = GetEventTypeSelectList();
            ViewBag.Statuses = GetStatusSelectList();
            return View();
        }

        public IEnumerable<SelectListItem> GetEventTypeSelectList()
        {
            return _context.EventTypes
                .Select(e => new SelectListItem { Value = e.EventTypeID.ToString(), Text = e.EventTypeName })
                .ToList();
        }

        public string GetStatusName(int statusID)
        {
            var status = _context.VolunteerStatuses.Find(statusID);
            return status?.Status;
        }

        public IEnumerable<SelectListItem> GetStatusSelectList()
        {
            return _context.VolunteerStatuses
                .Select(e => new SelectListItem { Value = e.VolunteerStatusID.ToString(), Text = e.Status })
                .ToList();
        }

        public string GetEventTypeName(int eventTypeId)
        {
            var eventType = _context.EventTypes.Find(eventTypeId);
            return eventType?.EventTypeName;
        }

        // POST: Volunteers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistrationVolunteer model)
        {

            string vkPattern = @"^(https?:\/\/)?(www\.)?vk\.com\/\w+$";
            string telegramPattern = @"^@([A-Za-z0-9_]{1,15})$";

            // Проверяем, соответствует ли ввод пользователя шаблону ссылки VK или имени пользователя Telegram
            if (!string.IsNullOrEmpty(model.VK) && !Regex.IsMatch(model.VK, vkPattern))
            {
                ModelState.AddModelError("VK", "Введите корректную ссылку на VK в формате 'https://vk.com'");
            }
            if (!string.IsNullOrEmpty(model.Telegram) && !Regex.IsMatch(model.Telegram, telegramPattern))
            {
                ModelState.AddModelError("Telegram", "Введите корректное имя пользователя Telegram (начинается с @)");
            }
            if (model.BirthDate.Year < 1924 || model.BirthDate.Year > 2009)
            {
                ModelState.AddModelError("BirthDate", "Введите корректную дату рождения");
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                Volunteer volunteer = new Volunteer
                {
                    UserID = user.Id,
                    Name = model.Name,
                    Patronymic = model.Patronymic,
                    Surname = model.Surname,
                    BirthDate = model.BirthDate,
                    EventTypeID = model.EventTypeID,
                    Telegram = model.Telegram,
                    VK = model.VK,
                    VolunteerStatusID = model.VolunteerStatusID
                };

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Volunteer");
                    _context.Add(volunteer);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            return View(model);
        }

        // GET: Volunteers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }
            return View(volunteer);
        }

        // POST: Volunteers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VolunteerID,UserID,Name,Patronymic,Surname,BirthDate,EventTypeID,VolunteerStatusID,VK,Telegram")] Volunteer volunteer)
        {
            if (id != volunteer.VolunteerID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(volunteer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VolunteerExists(volunteer.VolunteerID))
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
            return View(volunteer);
        }

        // GET: Volunteers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _context.Volunteers
                .FirstOrDefaultAsync(m => m.VolunteerID == id);
            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // POST: Volunteers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer != null)
            {
                _context.Volunteers.Remove(volunteer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VolunteerExists(int id)
        {
            return _context.Volunteers.Any(e => e.VolunteerID == id);
        }
    }
}
