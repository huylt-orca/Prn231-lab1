using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ODataBookStore.Models;
using ODataBookStore.Request;
using ODataBookStore.Utils;
using System.Security.Claims;

namespace ODataBookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly MyDBContext _context;
        public UserController(JwtService jwtService, MyDBContext context)
        {
            this._jwtService = jwtService;
            this._context = context;
        }
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(LoginRequest loginRequest)
        {
            User user =  _context.Users.FirstOrDefaultAsync(e => e.Username == loginRequest.UserName).Result;
            if (user == null)
            {
                return BadRequest();
            }
            bool isVerify = PasswordService.Verify(loginRequest.Password, user.Password);
            if (isVerify)
            {
                string token = _jwtService.GenerateJwtToken(user.Username, user.Role == Role.ADMIN ? "admin" : "user", Convert.ToString(user.Id));
                return Ok(token);
            }
            return Unauthorized();
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp (User user)
        {
            try
            {
                user.Password = PasswordService.Hash(user.Password);
                _context.Users.AddAsync(user);
                _context.SaveChangesAsync();

                return Ok(user);
            } catch (Exception e)
            {
                return BadRequest(e);
            }
        }


        /// <summary>
        /// Test Admin 
        /// </summary>

        [HttpGet("TestAdmin")]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> testAdmin()
        {
            return Ok("Admin 1");
        }

        [HttpGet("TestUser")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> testUser()
        {
            ClaimsPrincipal currentUser = HttpContext.User;
            string userId = currentUser.FindFirstValue("Id");
            return Ok("User 1 " + userId);
        }

        [HttpGet("Infor")]
        [Authorize]
        public async Task<IActionResult> GetInfor()
        {
            ClaimsPrincipal currentUser = HttpContext.User;
            int userId = Convert.ToInt32(currentUser.FindFirstValue("Id"));
            return Ok(_context.Users.FirstOrDefaultAsync(e => e.Id == userId).Result);
        }
    }
}
