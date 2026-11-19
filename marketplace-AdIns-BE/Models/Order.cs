using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using marketplace_AdIns_BE.Models;

namespace marketplace_AdIns_BE.Models
{
    public class Order
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey(nameof(User))]
        public long UserId { get; set; }
        public User? User { get; set; }

        [ForeignKey(nameof(Store))]
        public long StoreId { get; set; }
        public Store? Store { get; set; }

        [ForeignKey(nameof(Address))]
        public long AddressId { get; set; }
        public Address? Address { get; set; }

        [ForeignKey(nameof(Voucher))]
        public long? VoucherId { get; set; }
        public Voucher? Voucher { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? DiscountAmount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalBeforeDisc { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? TotalAfterDisc { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.PENDING_PAYMENT;

        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
