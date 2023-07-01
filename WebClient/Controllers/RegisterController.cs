using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class RegisterController : Controller
    {
        private readonly HttpClient client = null;
        private string UserApiUrl = "";

        public RegisterController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            UserApiUrl = "https://localhost:7065/api/User";
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserModel user)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(UserApiUrl + "/SignUp",content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Login");
                }
                return View();
            }
            catch
            {
                return View();
            }
        }
    }
}
