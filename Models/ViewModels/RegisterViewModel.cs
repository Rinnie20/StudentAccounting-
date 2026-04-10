using System.ComponentModel.DataAnnotations;

namespace StudentAccounting.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Выберите роль")]
        public string Role { get; set; } = "Manager"; // по умолчанию Manager
    }
}