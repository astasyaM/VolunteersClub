using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VolunteersClub.Models;
using VolunteersClub.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;

namespace VolunteersClub.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        // Проверяем роль пользователя
                        if (await _userManager.IsInRoleAsync(user, "Leader"))
                        {
                            return RedirectToAction("Details", "Leaders", new { id = user.Id});
                        }
                        else if (await _userManager.IsInRoleAsync(user, "Volunteer"))
                        {
                            return RedirectToAction("Details", "Volunteers", new { id = user.Id});
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ошибка аутентификации. Пожалуйста, проверьте корректность введённых данных.");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
