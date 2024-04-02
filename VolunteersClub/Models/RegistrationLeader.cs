using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace VolunteersClub.Models
{
    public class RegistrationLeader
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [CustomPassword(ErrorMessage = "Пароль не соответствует требованиям.")]
        [StringLength(100, ErrorMessage = "Пароль должен быть длиной от 8 до 100 символов.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Введённые пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Patronymic { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Confirm { get; set; }
    }
}
