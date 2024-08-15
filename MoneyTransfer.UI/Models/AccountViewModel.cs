using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace MoneyTransfer.UI.Models
{
    public class AccountViewModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(16)]
        [Required]
        [DisplayName("Hesap Numarası")]
        public string? AccountNumber { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        [Required]
        [DisplayName("Bakiye")]
        public decimal Balance { get; set; }

        [MaxLength(3)]
        [Required]
        [DisplayName("Para Birimi")]
        public string? AccountCurrency { get; set; }

        [Required]
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [JsonIgnore]
        public CustomerViewModel? Customer { get; set; }

        [JsonIgnore]
        public ICollection<TransferViewModel>? TransfersFrom { get; set; }

        [JsonIgnore]
        public ICollection<TransferViewModel>? TransfersTo { get; set; }
    }
}
