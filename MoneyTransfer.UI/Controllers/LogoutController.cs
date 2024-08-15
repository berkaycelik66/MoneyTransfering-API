using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace MoneyTransfer.UI.Controllers
{
    public class LogoutController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                HttpContext.Session.Clear();
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                Response.Cookies.Delete("token");
                TempData["successMessage"] = "Çıkış Başarılı";
            }
            catch
            {
                TempData["errorMessage"] = "Bir Hata Oluştu";
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
