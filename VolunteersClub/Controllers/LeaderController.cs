using Microsoft.AspNetCore.Mvc;
using VolunteersClub.Data;

namespace VolunteersClub.Controllers
{
    public class LeaderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
