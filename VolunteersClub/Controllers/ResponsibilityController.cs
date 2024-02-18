using Microsoft.AspNetCore.Mvc;
using VolunteersClub.Data;

namespace VolunteersClub.Controllers
{
    public class ResponsibilityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResponsibilityController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
