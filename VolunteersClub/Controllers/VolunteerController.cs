﻿using Microsoft.AspNetCore.Mvc;
using VolunteersClub.Data;

namespace VolunteersClub.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VolunteerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
