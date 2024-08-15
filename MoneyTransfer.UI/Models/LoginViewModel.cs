using System.ComponentModel;

namespace MoneyTransfer.UI.Models
{
    public class LoginViewModel
    {
        [DisplayName("TC/Mail/Telefon Numarası")]
        public string Username { get; set; }

        [DisplayName("Şifre")]
        public string Password { get; set; }
    }
}
