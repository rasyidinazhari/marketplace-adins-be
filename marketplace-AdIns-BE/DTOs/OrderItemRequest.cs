using System.ComponentModel.DataAnnotations;
namespace marketplace_AdIns_BE.DTOs
{
    public class OrderItemRequest
    {
        public long OrderId { get; set; }
        public long ProductId { get; set; }
        public long Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
