using System.ComponentModel.DataAnnotations;

namespace ServiceStub.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Введите логин")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; }
    }
}
