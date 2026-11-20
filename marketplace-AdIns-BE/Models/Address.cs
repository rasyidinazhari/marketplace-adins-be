using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace marketplace_AdIns_BE.Models
{
    public class Address
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public string RecipientName { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty ;

        [Required]
        public string City { get; set; } = string .Empty ;

        [Required]
        public string PostalCode { get; set; } = string.Empty;

        public string? Note { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Order> Orders { get; set; } = new List<Order>();


    }
}
