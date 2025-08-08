using System.ComponentModel.DataAnnotations;

namespace ServiceStub.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Введите логин")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Введите пароль")]
        public string ConfirmPassword { get; set; }
    }
}
