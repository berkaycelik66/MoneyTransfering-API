using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace MoneyTransfer.UI.Models
{
    public class CustomerViewModel
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(11)]
        public string? TC { get; set; }

        [Required]
        [MaxLength(50)]
        [DisplayName("İsim")]
        public string? Name { get; set; }

        [Required]
        [MaxLength(50)]
        [DisplayName("Soyisim")]
        public string? Surname { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Mail { get; set; }

        [Required]
        [MaxLength(50)]
        [DisplayName("Şifre")]
        public string? Password { get; set; }

        [Required]
        [MaxLength(11)]
        [DisplayName("Telefon Numarası")]
        public string? PhoneNumber { get; set; }

        [Required]
        [MaxLength(250)]
        [DisplayName("Adres")]
        public string? Address { get; set; }

        [JsonIgnore]
        public ICollection<AccountViewModel>? Accounts { get; set; } = new List<AccountViewModel>();

        [JsonIgnore]
        public string? RefreshToken { get; set; }

        [JsonIgnore]
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
