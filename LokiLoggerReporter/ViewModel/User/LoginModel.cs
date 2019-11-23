using System.ComponentModel.DataAnnotations;

namespace lokiloggerreporter.ViewModel.User
{
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}