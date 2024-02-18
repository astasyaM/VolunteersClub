using Microsoft.AspNetCore.Mvc;
using System.Runtime.ExceptionServices;
using VolunteersClub.Data;
using VolunteersClub.Models;

namespace VolunteersClub.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Event> events = _context.Events.ToList();
            return View(events);
        }

        public IActionResult Detail(int id)
        {
            Event @event = _context.Events.FirstOrDefault(c => c.EventID == id);
            return View(@event);
        }
    }
}
