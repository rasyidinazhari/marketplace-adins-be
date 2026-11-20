using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace marketplace_AdIns_BE.Models
{
    public class Product
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey(nameof(Store))]
        public long StoreId { get; set; }
        public Store? Store { get; set; }

        public string? ProductName { get; set; } = string.Empty;
        public string? ProductDescription { get; set; } = string.Empty ;

        [Column(TypeName = "decimal(12,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Harga Minimal 0")]
        public decimal? ProductPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stok Minimal 0")]
        public int? ProductStock { get; set; } = 0;

        public bool? isActive { get; set; } = true;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    }
}
