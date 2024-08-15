using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace MoneyTransfer.UI.Models
{
    public class TransferViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Account")]
        [DisplayName("GönderenId")]
        public int AccountFromId { get; set; }

        [Required]
        [ForeignKey("Account")]
        [DisplayName("Alıcı Hesap Numarası")]
        public string? ToAccountNumber { get; set; }

        [DisplayName("Gönderen")]
        public string? FromCustomerName { get; set; }

        [DisplayName("Alıcı")]
        public string? ToCustomerName { get; set; }

        [Required]
        [Column(TypeName = "decimal(13,2)")]
        [DisplayName("Tutar")]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName = "decimal(13,2)")]
        [DisplayName("Transfer Ücreti")]
        public decimal Fee { get; set; }

        [Required]
        [MaxLength(3)]
        [DisplayName("Para Birimi")]
        public string? TransferCurrency { get; set; }

        [Required]
        [DisplayName("Tarih")]
        public DateTime Date { get; set; }

        [MaxLength(100)]
        [Required]
        [DisplayName("Açıklama")]
        public string? Description { get; set; }

        [JsonIgnore]
        public AccountViewModel? AccountFrom { get; set; }

        [JsonIgnore]
        public AccountViewModel? AccountTo { get; set; }
    }
}
