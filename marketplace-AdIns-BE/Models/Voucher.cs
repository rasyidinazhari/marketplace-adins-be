using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace marketplace_AdIns_BE.Models
{
    public class Voucher
    {
        [Key]
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public long StoreId { get; set; }
        public Store? Store { get; set; } = new();

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public int DiscountPercentage { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public int MinPurchaseAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public int MaxDiscountAmount { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set;} = DateTime.Now;

        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
