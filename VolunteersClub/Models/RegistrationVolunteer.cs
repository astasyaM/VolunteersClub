using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace VolunteersClub.Models
{
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
        public string Patronymic { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public DateOnly BirthDate { get; set; }
        [Required]
        public int EventTypeID { get; set; }
        [Required]
        public int VolunteerStatusID { get; set; }
        public virtual VolunteerStatus VolunteerStatus { get; set; }
        public string VK { get; set; }
        public string Telegram { get; set; }
    }
}
