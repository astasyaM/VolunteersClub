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
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        // GET: Events
        public async Task<IActionResult> IndexMarks()
        {
            var currentDate = DateTime.Today;

            var pastEvents = await _context.Events
                .Where(p => p.EventDate.Year < currentDate.Year ||
                p.EventDate.Year == currentDate.Year && p.EventDate.Month < currentDate.Month ||
                p.EventDate.Year == currentDate.Year && p.EventDate.Month == currentDate.Month && p.EventDate.Day < currentDate.Day)
                .ToListAsync();
            return View(pastEvents);
        }

        // GET: Events
        public async Task<IActionResult> IndexVolunteers(string volunteerId, int? eventType, DateTime? startDate, DateTime? endDate)
        {
            ViewBag.Id = volunteerId;

            var query = _context.Events.AsQueryable();

            // Применяем фильтр по типу мероприятия, если выбрано
            if (eventType.HasValue && eventType != 0)
            {
                query = query.Where(e => e.EventTypeID == eventType);
            }

            // Применяем фильтр по дате начала, если указана
            if (startDate.HasValue)
            {
                query = query.Where(p => p.EventDate.Year < endDate.Value.Year ||
                p.EventDate.Year == endDate.Value.Year && p.EventDate.Month < endDate.Value.Month ||
                p.EventDate.Year == endDate.Value.Year && p.EventDate.Month == endDate.Value.Month && p.EventDate.Day < endDate.Value.Day);
            }

            if (startDate.HasValue)
            {
                query = query.Where(p => p.EventDate.Year > startDate.Value.Year ||
                p.EventDate.Year == startDate.Value.Year && p.EventDate.Month > startDate.Value.Month ||
                p.EventDate.Year == startDate.Value.Year && p.EventDate.Month == startDate.Value.Month && p.EventDate.Day > startDate.Value.Day);
            }

            var events = query.ToList();
            var eventTypes = _context.EventTypes.ToList();

            var viewModel = new ViewEventForVolunteers
            {
                Events = events,
                EventTypes = eventTypes
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult MakeRequest(int eventId, string volunteerId)
        {
            try
            {
                var volunteerUser = _context.Volunteers.FirstOrDefault(m => m.UserID == volunteerId);

                if (volunteerUser != null)
                {
                    var participant = new Participant
                    {
                        EventID = eventId,
                        VolunteerID = volunteerUser.VolunteerID,
                        ConfirmedLeader = false,
                        ConfirmedVolunteer = true,
                        ResponsibilityID = 1
                    };
                    _context.Add(participant);
                    _context.SaveChanges();
                }

                // Возвращаем успешный результат
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Возвращаем ошибку в случае исключения
                return Json(new { success = false, error = ex.Message });
            }
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventID == id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewBag.EventTypeName = GetEventTypeName(@event.EventTypeID);
            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewBag.EventTypes = GetEventTypeSelectList();
            return View();
        }

        public IEnumerable<SelectListItem> GetEventTypeSelectList()
        {
            return _context.EventTypes
                .Select(e => new SelectListItem { Value = e.EventTypeID.ToString(), Text = e.EventTypeName })
                .ToList();
        }

        public string GetEventTypeName(int eventTypeId)
        {
            var eventType = _context.EventTypes.Find(eventTypeId);
            return eventType?.EventTypeName;
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventID,EventName,EventDate,StartTime,Duration,VolunteersNumber,EventTypeID,Adress,EventDescription,Image")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewBag.EventTypes = GetEventTypeSelectList();
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventID,EventName,EventDate,StartTime,Duration,VolunteersNumber,EventTypeID,Adress,EventDescription,Image")] Event @event)
        {
            if (id != @event.EventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventID))
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
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventID == id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewBag.EventTypeName = GetEventTypeName(@event.EventTypeID);
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }
    }
}
