using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using WebClient.Models;
using System.Text;
using NuGet.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebClient.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient client = null;
        private string UserApiUrl = "";

        public LoginController()
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
        public async Task<IActionResult> Index(LoginModel acc)
        {
            try
            {
                if (acc.Username == null || acc.Password == null) return View();

                var content = new StringContent(JsonConvert.SerializeObject(acc), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(UserApiUrl+"/SignIn",content);

                if (response.IsSuccessStatusCode)
                {
                    string strData = await response.Content.ReadAsStringAsync();
                    string item = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(strData));

                    HttpContext.Session.SetString("token", item.Trim('\"')); 

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", item.Trim('\"'));
                    HttpResponseMessage responseInfor = await client.GetAsync(UserApiUrl+ "/Infor");
                    string strDataUser = await responseInfor.Content.ReadAsStringAsync();

                    var parseUser = JObject.Parse(strDataUser);
                    var user = JsonConvert.DeserializeObject<UserModel>(JsonConvert.SerializeObject(parseUser));
                    HttpContext.Session.SetString("currentUser", JsonConvert.SerializeObject(user));

                    switch (user.Role)
                    {
                        case Role.User:
                            return RedirectToAction("Index", "Book");
                        case Role.Admin:
                            return RedirectToAction("Index", "Book");
                        default:
                            return View();
                    }
                }
                else
                {
                    ViewBag.Message = "Username or Password is wrong.";
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
