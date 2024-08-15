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
    public class Transfer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Account")]
        public int AccountFromId { get; set; }

        [Required]
        [ForeignKey("Account")]
        public int AccountToId { get; set; }

        [Required]
        [Column(TypeName = "decimal(13,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName = "decimal(13,2)")]
        public decimal Fee { get; set; }

        [Required]
        [MaxLength(3)]
        public string? TransferCurrency { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [MaxLength(100)]
        [Required]
        public string? Description { get; set; }

        [JsonIgnore]
        public Account? AccountFrom { get; set; }

        [JsonIgnore]
        public Account? AccountTo { get; set; }
    }
}
