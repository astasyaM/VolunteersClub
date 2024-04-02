using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VolunteersClub.Models;
using VolunteersClub.Data;
using Microsoft.EntityFrameworkCore;

namespace VolunteersClub.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                if (result.Succeeded)
                {
                    // Успешная аутентификация, перенаправление на указанную страницу
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ошибка аутентификации. Пожалуйста, проверьте корректность введённых данных.");
                }
            }

            // Если произошла ошибка в модели, возвращаем представление снова с моделью
            return View(model);
        }
    }
}
