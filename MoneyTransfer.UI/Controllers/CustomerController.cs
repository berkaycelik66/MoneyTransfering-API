using Microsoft.AspNetCore.Mvc;
using MoneyTransfer.UI.Models;
using Newtonsoft.Json;
using NuGet.Common;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Globalization;

namespace MoneyTransfer.UI.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5056/api");
        private readonly HttpClient _httpClient;

        public CustomerController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() 
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register(CustomerViewModel model)
        {
            string data = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(data , Encoding.UTF8, "application/json");

            HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "/Customer/CreateCustomer", content).Result;

            string data2 = response.Content.ReadAsStringAsync().Result;

            ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(data2)!;

            if (response.IsSuccessStatusCode) 
            {
                TempData["successMessage"] = apiResponse?.Message;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["errorMessage"] = apiResponse?.Message;
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login(LoginViewModel model)
        {
            string data = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "/login", content).Result;

            string data2 = response.Content.ReadAsStringAsync().Result;

            ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(data2)!;

            if (response.IsSuccessStatusCode)
            {
                var tokens = JsonConvert.DeserializeObject<TokenResponseModel>(apiResponse?.Data.ToString()!);
                if (tokens != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim("accessToken", tokens.AccessToken),
                        new Claim("refreshToken", tokens.RefreshToken)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.Now.AddMinutes(30) //Cookie Expiration Time
                    };

                    HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );

                    HttpContext.Session.SetString("accessToken", tokens.AccessToken.ToString());//key ve value 
                    HttpContext.Session.SetString("refreshToken", tokens.RefreshToken.ToString());//key ve value 
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken.ToString());

                    var tokenValue = tokens.AccessToken.ToString();
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddHours(1),
                        HttpOnly = true, // Make the cookie inaccessible to JavaScript
                        Secure = true // Ensure the cookie is sent over HTTPS
                    };

                    Response.Cookies.Append("token", tokenValue, cookieOptions);

                    TempData["successMessage"] = apiResponse?.Message;

                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["errorMessage"] = apiResponse?.Message;
            return View();
        }

        public class TokenResponseModel
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }
    }
}
