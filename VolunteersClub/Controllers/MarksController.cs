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
    public class MarksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MarksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Marks
        public async Task<IActionResult> Index(string id)
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

            
            // Предполагаем, что в таблице Participants есть столбец EventID, связывающий с Events
            var showMarks = await _context.Marks
                .Join(_context.Participants, // Присоединяем Participants
                      mark => mark.ActivityRecordID,
                      participant => participant.RecordID,
                      (mark, participant) => new { Mark = mark, Participant = participant })
                .Join(_context.Events, // Присоединяем Events
                      markAndParticipant => markAndParticipant.Participant.EventID,
                      eventEntity => eventEntity.EventID,
                      (markAndParticipant, eventEntity) => new { markAndParticipant.Mark, markAndParticipant.Participant, Event = eventEntity })
                .Where(x => x.Participant.VolunteerID == volunteer.VolunteerID) // Фильтр по ID волонтёра
                .Select(x => new MarkWithEventViewModel // Предполагается, что у вас есть такая ViewModel
                {
                    MarkID = x.Mark.MarkID,
                    CurrentMark = x.Mark.CurrentMark,
                    Notes = x.Mark.Notes,
                    EventName = x.Event.EventName, // Имя мероприятия
                    EventID = x.Event.EventID
                })
                .ToListAsync();
            ViewBag.VId = volunteer.VolunteerID;
            // Возвращаем список мероприятий с оценками
            return View(showMarks);
        }

        public async Task<IActionResult> IndexEvent(int id)
        {
            var currentEvent = await _context.Events
                .FirstOrDefaultAsync(m => m.EventID == id);
            if (currentEvent == null)
            {
                return NotFound();
            }

            var showMarks = await _context.Marks
                .Join(_context.Participants, // Присоединяем Participants
                      mark => mark.ActivityRecordID,
                      participant => participant.RecordID,
                      (mark, participant) => new { Mark = mark, Participant = participant })
                .Join(_context.Volunteers, 
                      markAndParticipant => markAndParticipant.Participant.VolunteerID,
                      volunteerEntity => volunteerEntity.VolunteerID,
                      (markAndParticipant, volunteerEntity) => new { markAndParticipant.Mark, markAndParticipant.Participant, Volunteer = volunteerEntity })
                .Where(x => x.Participant.EventID == currentEvent.EventID) 
                .Select(x => new MarksWithVolunteerModel
                {
                    MarkID = x.Mark.MarkID,
                    CurrentMark = x.Mark.CurrentMark,
                    Notes = x.Mark.Notes,
                    VolunteerName = x.Volunteer.Name,
                    VolunteerSurname = x.Volunteer.Surname,
                    VolunteerID = x.Volunteer.UserID
                })
                .ToListAsync();

            // Возвращаем список мероприятий с оценками
            return View(showMarks);
        }

        // GET: Marks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var markWithEvent = await _context.Marks
                .Where(m => m.MarkID == id) // Фильтруем Marks по ID
                .Join(_context.Participants, // Присоединяем Participants
                      mark => mark.ActivityRecordID,
                      participant => participant.RecordID,
                      (mark, participant) => new { Mark = mark, Participant = participant })
                .Join(_context.Events, // Присоединяем Events
                      markAndParticipant => markAndParticipant.Participant.EventID,
                      eventEntity => eventEntity.EventID,
                      (markAndParticipant, eventEntity) => new MarkWithEventViewModel // Создаём экземпляр ViewModel
                      {
                          MarkID = markAndParticipant.Mark.MarkID,
                          CurrentMark = markAndParticipant.Mark.CurrentMark,
                          Notes = markAndParticipant.Mark.Notes,
                          EventName = eventEntity.EventName 
                      })
                .FirstOrDefaultAsync();

            if (markWithEvent == null)
            {
                return NotFound();
            }

            return View(markWithEvent);
        }

        // GET: Marks/Create
        public IActionResult Create(int id)
        {
            ViewBag.ID = id;
            return View();
        }

        // POST: Marks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ActivityRecordID, CurrentMark, Note")] Mark mark)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mark);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mark);
        }

        // GET: Marks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mark = await _context.Marks.FindAsync(id);
            if (mark == null)
            {
                return NotFound();
            }
            return View(mark);
        }

        // POST: Marks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MarkID,ActivityRecordID,CurrentMark,Note")] Mark mark)
        {
            if (id != mark.MarkID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mark);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarkExists(mark.MarkID))
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
            return View(mark);
        }

        // GET: Marks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mark = await _context.Marks
                .FirstOrDefaultAsync(m => m.MarkID == id);
            if (mark == null)
            {
                return NotFound();
            }

            return View(mark);
        }

        // POST: Marks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mark = await _context.Marks.FindAsync(id);
            if (mark != null)
            {
                _context.Marks.Remove(mark);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarkExists(int id)
        {
            return _context.Marks.Any(e => e.MarkID == id);
        }
    }
}
