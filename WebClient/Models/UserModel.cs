

namespace WebClient.Models
{
    public enum Role
    {
        Admin, User
    }
    public class UserModel
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}
