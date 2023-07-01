using Swashbuckle.AspNetCore.Annotations;

namespace ODataBookStore.Request
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
