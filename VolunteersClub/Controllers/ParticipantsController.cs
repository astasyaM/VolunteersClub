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
    public class ParticipantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParticipantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class ConfirmedVolunteerViewModel
        {
            public int RecordID {  get; set; }
            public int VolunteerId { get; set; }
            public string Surname { get; set; }
            public string Name {  get; set; }
            public string Status {  get; set; }
            public string Patronymic {  get; set; }
            public DateOnly BirthDate {  get; set; }
            public string VK {  get; set; }
            public string Telegram { get; set; }
        }


        // GET: Participants
        public async Task<IActionResult> Index()
        {
            var participants = _context.Participants.ToList();

            var confirmedVolunteer = _context.Participants
                .Where(p => p.ConfirmedVolunteer && !p.ConfirmedLeader)
                .Include(p => p.Volunteer)  // Включаем данные о волонтёре
                .Select(p => new ConfirmedVolunteerViewModel
                {
                    RecordID = p.RecordID,
                    VolunteerId = p.VolunteerID,
                    Surname = p.Volunteer.Surname,
                    Name = p.Volunteer.Name,
                    Patronymic = p.Volunteer.Patronymic,
                    Status = p.Volunteer.VolunteerStatus.Status,
                    BirthDate = p.Volunteer.BirthDate,
                    VK = p.Volunteer.VK,
                    Telegram = p.Volunteer.Telegram
                    
                })
                .ToList();

            ViewBag.ConfirmedVolunteers = confirmedVolunteer;

            var confirmedLeader = _context.Participants
                .Where(p => p.ConfirmedLeader && !p.ConfirmedVolunteer)
                .Include(p => p.Volunteer)  // Включаем данные о волонтёре
                .Select(p => new ConfirmedVolunteerViewModel
                {
                    RecordID = p.RecordID,
                    VolunteerId = p.VolunteerID,
                    Surname = p.Volunteer.Surname,
                    Name = p.Volunteer.Name,
                    Patronymic = p.Volunteer.Patronymic,
                    Status = p.Volunteer.VolunteerStatus.Status,
                    BirthDate = p.Volunteer.BirthDate,
                    VK = p.Volunteer.VK,
                    Telegram = p.Volunteer.Telegram

                })
                .ToList();

            ViewBag.ConfirmedLeader = confirmedLeader;

            var confirmedBoth = _context.Participants
                .Where(p => p.ConfirmedVolunteer & p.ConfirmedLeader)
                .Include(p => p.Volunteer)  // Включаем данные о волонтёре
                .Select(p => new ConfirmedVolunteerViewModel
                {
                    RecordID = p.RecordID,
                    VolunteerId = p.VolunteerID,
                    Surname = p.Volunteer.Surname,
                    Name = p.Volunteer.Name,
                    Patronymic = p.Volunteer.Patronymic,
                    Status = p.Volunteer.VolunteerStatus.Status,
                    BirthDate = p.Volunteer.BirthDate,
                    VK = p.Volunteer.VK,
                    Telegram = p.Volunteer.Telegram

                })
                .ToList();

            ViewBag.ConfirmedBoth = confirmedBoth;
            var nonParticipants = _context.Volunteers
                .Where(v => !_context.Participants.Any(p => p.VolunteerID == v.VolunteerID))
                .Include(v => v.VolunteerStatus)  // Включаем данные о статусе волонтёра
                .Select(v => new ConfirmedVolunteerViewModel
                {
                    VolunteerId = v.VolunteerID,
                    Surname = v.Surname,
                    Name = v.Name,
                    Patronymic = v.Patronymic,
                    Status = v.VolunteerStatus.Status,
                    BirthDate = v.BirthDate,
                    VK = v.VK,
                    Telegram = v.Telegram
                })
                .ToList();

            ViewBag.NonParticipants = nonParticipants;

            return View(participants);
        }

        [HttpPost]
        public IActionResult ApproveRequest(int participantId)
        {
            try
            {
                // Находим участника по ID
                var participant = _context.Participants.Find(participantId);

                if (participant != null)
                {
                    // Обновляем поле ConfirmedLeader
                    participant.ConfirmedLeader = true;

                    // Сохраняем изменения в базе данных
                    _context.SaveChanges();

                    // Возвращаем успешный результат
                    return Json(new { success = true });
                }
                else
                {
                    // Возвращаем ошибку, если участник не найден
                    return Json(new { success = false, error = "Participant not found" });
                }
            }
            catch (Exception ex)
            {
                // Возвращаем ошибку в случае исключения
                return Json(new { success = false, error = ex.Message });
            }
        }

        // GET: Participants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participant = await _context.Participants
                .FirstOrDefaultAsync(m => m.RecordID == id);
            if (participant == null)
            {
                return NotFound();
            }

            return View(participant);
        }

        // GET: Participants/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Participants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecordID,EventID,VolunteerID,ResponsibilityID,ConfirmedVolunteer,ConfirmedLeader")] Participant participant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(participant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(participant);
        }

        // GET: Participants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participant = await _context.Participants.FindAsync(id);
            if (participant == null)
            {
                return NotFound();
            }
            return View(participant);
        }

        // POST: Participants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecordID,EventID,VolunteerID,ResponsibilityID,ConfirmedVolunteer,ConfirmedLeader")] Participant participant)
        {
            if (id != participant.RecordID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(participant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParticipantExists(participant.RecordID))
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
            return View(participant);
        }

        // GET: Participants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participant = await _context.Participants
                .FirstOrDefaultAsync(m => m.RecordID == id);
            if (participant == null)
            {
                return NotFound();
            }

            return View(participant);
        }

        // POST: Participants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant != null)
            {
                _context.Participants.Remove(participant);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParticipantExists(int id)
        {
            return _context.Participants.Any(e => e.RecordID == id);
        }
    }
}
