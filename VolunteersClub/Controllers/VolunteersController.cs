using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Drawing;

namespace VolunteersClub.Controllers
{
    public class VolunteersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public VolunteersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Volunteers
        public async Task<IActionResult> Index()
        {
            var volunteers = await _context.Volunteers
                .Include(v => v.EventType) // Загружаем связанный тип мероприятия
                .Include(v => v.VolunteerStatus) // Загружаем связанный статус волонтёра
                .ToListAsync();
            return View(volunteers);
        }

        public async Task<IActionResult> IndexForMarks(int id)
        {
            var listVolunteers = await _context.Participants
                .Where(m => m.EventID == id)
                .Select(m => m.VolunteerID)
                .ToListAsync();

            var volunteers = await _context.Volunteers
                .Include(v => v.EventType) 
                .Include(v => v.VolunteerStatus)
                .Join(_context.Participants,
                      volunteer => volunteer.VolunteerID,
                      participant => participant.VolunteerID, 
                      (volunteer, participant) => new { Volunteer = volunteer, Participant = participant })

                .Where(x => listVolunteers.Contains(x.Participant.VolunteerID) && x.Participant.EventID==id)
                .Select(x => new VolunteersForMarks
                {
                    RecordID = x.Participant.RecordID, 
                    BirthDate = x.Volunteer.BirthDate,
                    VolunteerID = x.Volunteer.VolunteerID,
                    EventType = x.Volunteer.EventType,
                    EventTypeID = x.Volunteer.EventTypeID,
                    Surname = x.Volunteer.Surname,
                    Name = x.Volunteer.Name,
                    Patronymic = x.Volunteer.Patronymic,
                    Telegram = x.Volunteer.Telegram,
                    UserID = x.Volunteer.UserID,
                    VK = x.Volunteer.VK,
                    VolunteerStatus = x.Volunteer.VolunteerStatus,
                    VolunteerStatusID = x.Volunteer.VolunteerStatusID
                })
                .ToListAsync();

            return View(volunteers);
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

            var currentDate = DateTime.Today;

            var pastEvents = await _context.Events
                .Where(p => p.EventDate.Year < currentDate.Year || 
                p.EventDate.Year==currentDate.Year && p.EventDate.Month < currentDate.Month || 
                p.EventDate.Year == currentDate.Year && p.EventDate.Month == currentDate.Month && p.EventDate.Day < currentDate.Day)
                .Select(p => p.EventID)
                .ToListAsync();

            var regularEventsCount = await _context.Participants
                .Where(p => p.VolunteerID == volunteer.VolunteerID && 
                p.ResponsibilityID==1 &&
                p.ConfirmedLeader==true &&
                p.ConfirmedVolunteer==true &&
                pastEvents.Contains(p.EventID))
                .Select(p => p.EventID)
                .Distinct()
                .CountAsync();

            var listRegularEventsCount = await _context.Participants
                .Where(p => p.VolunteerID == volunteer.VolunteerID &&
                p.ResponsibilityID == 1 &&
                p.ConfirmedLeader == true &&
                p.ConfirmedVolunteer == true &&
                pastEvents.Contains(p.EventID))
                .Select(p => p.RecordID)
                .ToListAsync();

            var specialEventsCount = await _context.Participants
                .Where(p => p.VolunteerID == volunteer.VolunteerID &&
                p.ResponsibilityID == 2 &&
                p.ConfirmedLeader == true &&
                p.ConfirmedVolunteer == true &&
                pastEvents.Contains(p.EventID))
                .Select(p => p.EventID)
                .Distinct()
                .CountAsync();

            var listSpecialEventsCount = await _context.Participants
                .Where(p => p.VolunteerID == volunteer.VolunteerID &&
                p.ResponsibilityID == 2 &&
                p.ConfirmedLeader == true &&
                p.ConfirmedVolunteer == true &&
                pastEvents.Contains(p.EventID))
                .Select(p => p.RecordID)
                .ToListAsync();

            var listRegularMarks = await _context.Marks
                .Where(m => listRegularEventsCount.Contains(m.ActivityRecordID))
                .ToListAsync();

            var listSpecialMarks = await _context.Marks
                .Where(m => listSpecialEventsCount.Contains(m.ActivityRecordID))
                .ToListAsync();

            var averageRatingRegular = Math.Round(listRegularMarks.Any() ? listRegularMarks.Average(m => m.CurrentMark) : 0, 2);
            var averageRatingSpecial = Math.Round(listSpecialMarks.Any() ? listSpecialMarks.Average(m => m.CurrentMark) : 0, 2);

            // Получаем общее количество мероприятий, в которых участвовал волонтёр
            int totalEventsCount = regularEventsCount + specialEventsCount;

            var allEvents = await _context.Participants
                .Where(p => p.VolunteerID == volunteer.VolunteerID &&
                p.ConfirmedLeader == true &&
                p.ConfirmedVolunteer == true)
                .Select(p => p.EventID)
                .ToListAsync();

            var futureEvents = await _context.Events
                .Where(p => (p.EventDate.Year > currentDate.Year ||
                p.EventDate.Year == currentDate.Year && p.EventDate.Month > currentDate.Month ||
                p.EventDate.Year == currentDate.Year && p.EventDate.Month == currentDate.Month && p.EventDate.Day > currentDate.Day) &&
                allEvents.Contains(p.EventID))
                .Select(g => new
                {
                    Name = _context.Events.FirstOrDefault(t => t.EventID == g.EventID).EventName,
                    Date = g.EventDate,
                    Time = g.StartTime,
                    Address = g.Adress
                })
                .ToListAsync();

            // Получаем данные о количестве мероприятий каждого типа
            var eventTypesData = await _context.Events
                .Where(e => allEvents.Contains(e.EventID) &&
                pastEvents.Contains(e.EventID)) 
                .GroupBy(e => e.EventTypeID) 
                .Select(g => new
                {
                    Type = _context.EventTypes.FirstOrDefault(t=>t.EventTypeID==g.Key).EventTypeName,
                    Count = g.Count(),
                    Percentage = Math.Round(((double)g.Count() / totalEventsCount) * 100, 2),
                    PercentageString = (Math.Round(((double)g.Count() / totalEventsCount) * 100, 2)).ToString()+"%",
                    Color = "#0d144a"
                })
                .ToListAsync();

            ViewBag.FutureEvents = futureEvents;
            ViewBag.EventTypesData = eventTypesData;
            ViewBag.CurrentVolunteer = volunteer;
            ViewBag.RegularEventsCount = regularEventsCount;
            ViewBag.SpecialEventsCount = specialEventsCount;
            ViewBag.AverageRatingRegular = averageRatingRegular;
            ViewBag.AverageRatingSpecial = averageRatingSpecial;

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
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Переадресация на личный кабинет
                    return RedirectToAction("Details", "Volunteers", new { id = volunteer.UserID });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            return View();
        }

        // GET: Volunteers/Edit/5
        public async Task<IActionResult> Edit(string? id)
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
            ViewBag.EventTypes = GetEventTypeSelectList();
            ViewBag.Statuses = GetStatusSelectList();
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
