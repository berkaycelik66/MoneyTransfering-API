using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoneyTransfer.Entities
{
    public class Customer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(11)]
        public string TC { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Surname { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Mail { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Password { get; set; }

        [Required]
        [MaxLength(11)]
        public string? PhoneNumber { get; set; }

        [Required]
        [MaxLength(250)]
        public string? Address { get; set; }

        [JsonIgnore]
        public ICollection<Account>? Accounts { get; set; } = new List<Account>();

        [JsonIgnore]
        public string? RefreshToken { get; set; }

        [JsonIgnore]
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}
