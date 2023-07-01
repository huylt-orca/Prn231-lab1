using System.ComponentModel.DataAnnotations;

namespace WebClient.Models
{
    public class LoginModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is Empty")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is Empty")]
        public string Password { get; set; }
    }
}
