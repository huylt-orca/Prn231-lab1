using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class BookController : Controller
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        private string PressApiUrl = "";
        public BookController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "https://localhost:7065/api/Book";
            PressApiUrl = "https://localhost:7065/api/Press";
        }
        public async Task<IActionResult> Index(string searchKeyword = "",int page = 1, int pageSize = 3)
        {
            string token = HttpContext.Session.GetString("token");
            if (token == null)
            {
                return RedirectToAction("Index", "Login");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string requestUrl = ProductApiUrl + "?$orderby=Id desc&$expand=Press,Location&$skip=" + (page-1) * pageSize;

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                requestUrl += $"&$filter=contains(Title, '{searchKeyword}')" +
                              $" or contains(Author, '{searchKeyword}')" +
                              $" or contains(ISBN, '{searchKeyword}')";

            }
            HttpResponseMessage response = await client.GetAsync(requestUrl);
            string strData = await response.Content.ReadAsStringAsync();

            var temp = JArray.Parse(strData);
            //    List < BookModel > items = (temp).Select(x => new BookModel
            //     {
            //        Id = (int)x["id"].ToObject<int>(),
            //        Author = (string)x["author"].ToObject<string>(),
            //        ISBN = (string)x["isbn"].ToObject<string>(),
            //        Title = (string)x["title"].ToObject<string>(),
            //        Price = (decimal)x["price"].ToObject<decimal>(),
            //        Press = new PressModel
            //        {
            //            Id = (int)x["press.id"].ToObject<int>(),
            //            Name = (string)x["press.name"].ToObject<string>(),
            //            Category = (Category)x["press.Category"].ToObject<Category>(),
            //        }
            //}).ToList();

            var items = JsonConvert.DeserializeObject<List<BookModel>>(JsonConvert.SerializeObject(temp));
            //TempData["BookList"] = items;

            //get total book
            string requestTotalUrl = ProductApiUrl + "/total";
            //if (searchKeyword != "") requestTotalUrl += ("?value=" + searchKeyword);
            HttpResponseMessage responseTotalBook = await client.GetAsync(requestTotalUrl + "?value=" + searchKeyword);
            string strDataTotalBook = await responseTotalBook.Content.ReadAsStringAsync();
            int tempTotalData = JsonConvert.DeserializeObject<int>(JsonConvert.SerializeObject(strDataTotalBook));

            ViewData["total"] = (int)Math.Ceiling((decimal)tempTotalData / pageSize);
            ViewData["currentPage"] = page;
            ViewData["searchValue"] = searchKeyword;
            return View(items);
        }

        public async Task<IActionResult> Create()
        {
            string token = HttpContext.Session.GetString("token");
            if (token == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var userObject = HttpContext.Session.GetString("currentUser");
            var user = JsonConvert.DeserializeObject<UserModel>(userObject);
            if (user.Role == Role.User)
            {
                return RedirectToAction(nameof(Index));
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.GetAsync(PressApiUrl);
            string strData = await response.Content.ReadAsStringAsync();

            var temp = JArray.Parse(strData);
            var items = JsonConvert.DeserializeObject<List<PressModel>>(JsonConvert.SerializeObject(temp));
            TempData["PressList"] = items;
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookModel product)
        {
            try
            {
                string token = HttpContext.Session.GetString("token");
                if (token == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                var userObject = HttpContext.Session.GetString("currentUser");
                var user = JsonConvert.DeserializeObject<UserModel>(userObject);
                if (user.Role == Role.User)
                {
                    return RedirectToAction(nameof(Index));
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json"); 

                HttpResponseMessage response = await client.PostAsync(ProductApiUrl, content); 
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Book");
                }
                else
                {
                    HttpResponseMessage responsePress = await client.GetAsync(PressApiUrl);
                    string strData = await responsePress.Content.ReadAsStringAsync();

                    var temp = JArray.Parse(strData);
                    var items = JsonConvert.DeserializeObject<List<PressModel>>(JsonConvert.SerializeObject(temp));
                    TempData["PressList"] = items;
                    return View();
                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            string token = HttpContext.Session.GetString("token");
            if (token == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var userObject = HttpContext.Session.GetString("currentUser");
            var user = JsonConvert.DeserializeObject<UserModel>(userObject);
            if (user.Role == Role.User)
            {
                return RedirectToAction(nameof(Index));
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.GetAsync(ProductApiUrl + "/" + id);
            string strData = await response.Content.ReadAsStringAsync();

            var temp = JObject.Parse(strData);

            var item = JsonConvert.DeserializeObject<BookModel>(JsonConvert.SerializeObject(temp));


            HttpResponseMessage responsePress = await client.GetAsync(PressApiUrl);
            string strDataPress = await responsePress.Content.ReadAsStringAsync();

            var tempPress = JArray.Parse(strDataPress);
            var items = JsonConvert.DeserializeObject<List<PressModel>>(JsonConvert.SerializeObject(tempPress));
            TempData["PressList"] = items;


            return View(item);
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookModel product)
        {
            try
            {
                string token = HttpContext.Session.GetString("token");
                if (token == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                var userObject = HttpContext.Session.GetString("currentUser");
                var user = JsonConvert.DeserializeObject<UserModel>(userObject);
                if (user.Role == Role.User)
                {
                    return RedirectToAction(nameof(Index));
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(ProductApiUrl + "/" + id, content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Book");
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            string token = HttpContext.Session.GetString("token");
            if (token == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var userObject = HttpContext.Session.GetString("currentUser");
            var user = JsonConvert.DeserializeObject<UserModel>(userObject);
            if (user.Role == Role.User)
            {
                return RedirectToAction(nameof(Index));
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.GetAsync(ProductApiUrl + "/" + id);
            string strData = await response.Content.ReadAsStringAsync();

            var temp = JObject.Parse(strData);

            var item = JsonConvert.DeserializeObject<BookModel>(JsonConvert.SerializeObject(temp));

            return View(item);
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, BookModel product)
        {
            try
            {
                string token = HttpContext.Session.GetString("token");

                if (token == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                var userObject = HttpContext.Session.GetString("currentUser");
                var user = JsonConvert.DeserializeObject<UserModel>(userObject);
                if (user.Role == Role.User)
                {
                    return RedirectToAction(nameof(Index));
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.DeleteAsync(ProductApiUrl + "/" + id);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Book");
                }
                else
                {
                    return View();
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
