using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace VolunteersClub.Models
{
    public class DateOnlyRangeAttribute : ValidationAttribute
    {

        public override bool IsValid(object value)
        {
            if (value is not DateOnly dateOnly)
                return false;

            return dateOnly.Year >= 1924 && dateOnly.Year <= 2009;
        }
    }

    public class CustomPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string password)
            {
                bool hasUpperCase = password.Any(char.IsUpper);
                bool hasLowerCase = password.Any(char.IsLower);
                bool hasDigits = password.Any(char.IsDigit);

                if (!hasUpperCase)
                    return new ValidationResult("Пароль должен содержать хотя бы одну заглавную букву.");
                if (!hasLowerCase)
                    return new ValidationResult("Пароль должен содержать хотя бы одну строчную букву.");
                if (!hasDigits)
                    return new ValidationResult("Пароль должен содержать хотя бы одну цифру.");

                return ValidationResult.Success;
            }

            return new ValidationResult("Неверный формат пароля.");
        }
    }

    public class RegistrationVolunteer
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
        public string Patronymic { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DateOnlyRange(ErrorMessage = "Дата рождения должна быть между 01.01.1924 и 01.01.2009")]
        public DateOnly BirthDate { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int EventTypeID { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int VolunteerStatusID { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string VK { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Telegram { get; set; }
    }
}
