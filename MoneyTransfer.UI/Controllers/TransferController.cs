using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MoneyTransfer.UI.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MoneyTransfer.UI.Controllers
{
    [Authorize]
    public class TransferController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5056/api");
        private readonly HttpClient _httpClient;

        public TransferController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }

        [HttpGet("/Transfer/Index/")]
        public IActionResult Index(int? accountId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var accessToken = HttpContext.Session.GetString("accessToken");

            if (string.IsNullOrEmpty(accessToken))
            {
                TempData["errorMessage"] = "Kullanıcı doğrulama bilgisi bulunamadı.";
                return RedirectToAction("Login", "Customer");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            AccountController _accountController = new AccountController();
            List<TransferViewModel> transfers = new List<TransferViewModel>();
            List<AccountViewModel> accounts = _accountController.GetAccounts(accessToken);

            string url = _httpClient.BaseAddress + $"/Transfer/AccountTransfers/{accountId}";

            if (startDate.HasValue)
            {
                url += $"?startDate={startDate.Value.ToString("yyyy-MM-dd")}";
            }

            if (endDate.HasValue)
            {
                url += $"{(startDate.HasValue ? "&" : "?")}endDate={endDate.Value.ToString("yyyy-MM-dd")}";
            }

            HttpResponseMessage response = _httpClient.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(data)!;
                transfers = JsonConvert.DeserializeObject<List<TransferViewModel>>(apiResponse.Data.ToString());
                transfers = transfers.OrderByDescending(t => t.Date).ToList();
            }
            ViewBag.Accounts = accounts;
            return View(transfers);
        }

        [HttpGet]
        public IActionResult MoneyTransfer()
        {
            var accessToken = HttpContext.Session.GetString("accessToken");

            if (string.IsNullOrEmpty(accessToken))
            {
                TempData["errorMessage"] = "Kullanıcı doğrulama bilgisi bulunamadı.";
                return RedirectToAction("Login", "Customer");
            }

            AccountController _accountController = new AccountController();
            List<AccountViewModel> accounts = _accountController.GetAccounts(accessToken);
            ViewBag.Accounts = accounts ?? new List<AccountViewModel>();

            return View();
        }

        public IActionResult MoneyTransfer(TransferRequestModel model)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("accessToken"));
            string data = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "/Transfer/moneyTransfer", content).Result;

            string data2 = response.Content.ReadAsStringAsync().Result;

            ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(data2)!;

            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = apiResponse?.Message;
                return RedirectToAction("Index", "Transfer");
            }
            else
            {
                TempData["errorMessage"] = apiResponse?.Message;
                return RedirectToAction("MoneyTransfer", "Transfer");
            }
        }


        public class TransferRequestModel
        {
            public int AccountFromId { get; set; }
            public string? ToAccountNumber { get; set; }
            public decimal Amount { get; set; }
            public string? Description { get; set; }
        }

    }
}
