using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoneyTransfer.Entities
{
    public class Account
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(16)]
        [Required]
        public string? AccountNumber { get; set; }

        [Column(TypeName = "decimal(13,2)")]
        [Required]
        public decimal Balance { get; set; }

        [MaxLength(3)]
        [Required]
        public string? AccountCurrency { get; set; }

        [Required]
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [JsonIgnore]
        public Customer? Customer { get; set; }

        [JsonIgnore]
        public ICollection<Transfer>? TransfersFrom { get; set; }

        [JsonIgnore]
        public ICollection<Transfer>? TransfersTo { get; set; }
    }
}
