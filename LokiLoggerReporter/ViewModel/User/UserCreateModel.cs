using System.ComponentModel.DataAnnotations;

namespace lokiloggerreporter.ViewModel.User
{
    public class UserCreateModel : UserUpdateModel
    {
        [Required(ErrorMessage = "Der Nutzer muss einen eindeutige Namen haben")]
        public string UserName { get; set; }
    }
}