using Microsoft.AspNetCore.Mvc;
using MoneyTransfer.UI.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;


namespace MoneyTransfer.UI.Controllers
{
    public class AccountController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5056/api");
        private readonly HttpClient _httpClient;
        public List<AccountViewModel> Account { get; set; }

        public AccountController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            Account = new List<AccountViewModel>();
        }


        [HttpGet]
        public IActionResult Index()
        {
            List<AccountViewModel> accounts = new List<AccountViewModel>();
            var accessToken = HttpContext.Session.GetString("accessToken");

            if (string.IsNullOrEmpty(accessToken))
            {
                TempData["errorMessage"] = "Kullanıcı doğrulama bilgisi bulunamadı.";
                return RedirectToAction("Login", "Customer");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Account/CustomerAccounts").Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(data)!;
                accounts = JsonConvert.DeserializeObject<List<AccountViewModel>>(apiResponse?.Data.ToString()!)!;
            }
            return View(accounts);
        }


        public List<AccountViewModel> GetAccounts(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Account/CustomerAccounts").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(data)!;
                return JsonConvert.DeserializeObject<List<AccountViewModel>>(apiResponse.Data.ToString()!)!;
            }

            return new List<AccountViewModel>();
        }
    }
}
