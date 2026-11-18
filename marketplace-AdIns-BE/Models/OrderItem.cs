using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace marketplace_AdIns_BE.Models
{
    public class OrderItem
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey(nameof(Order))]
        public long OrderId { get; set; }
        public Order? Order { get; set; }

        [ForeignKey(nameof(Product))]
        public long ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity Minimal 0")]
        public long Quantity { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        [Range(0, int.MaxValue, ErrorMessage = "PriceAtPurchase Minimal 0")]
        public long PriceAtPurchase { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}
