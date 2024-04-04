//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using VolunteersClub.Models;

//namespace VolunteersClub.Data
//{
//    public class HeaderData: ViewComponent
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<ApplicationUser> _userManager;

//        public HeaderData(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
//        {
//            _context = context;
//            _userManager = userManager;
//        }

//        public async Task<IViewComponentResult> InvokeAsync()
//        {
//            ApplicationUser user = null;

//            if (HttpContext.User.Identity.IsAuthenticated && HttpContext.User.Identity.AuthenticationType)
//            {
//                // Получаем пользователя
//                user = await _userManager.GetUserAsync(HttpContext.User);
//            }

//            var userId = user.Id;

//            var userData = await _context.Volunteers
//                .Where(u => u.Id == userId)
//                .Select(u => new UserData { /* заполнение модели данных пользователя */ })
//                .FirstOrDefaultAsync();

//            return View(userData); // Передаем модель с данными пользователя в представление компонента
//        }

//    }
//}
