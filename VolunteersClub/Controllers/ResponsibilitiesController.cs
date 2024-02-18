using Microsoft.AspNetCore.Mvc;
using VolunteersClub.Data;

namespace VolunteersClub.Controllers
{
    public class ResponsibilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResponsibilitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
