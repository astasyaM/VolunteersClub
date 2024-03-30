using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace VolunteersClub.Models
{
    public class RegistrationLeader
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [CustomPassword(ErrorMessage = "Пароль не соответствует требованиям.")]
        [StringLength(100, ErrorMessage = "Пароль должен быть длиной от 8 до 100 символов.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Введённые пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        [Required]
        public DateOnly BirthDate { get; set; }
    }
}
