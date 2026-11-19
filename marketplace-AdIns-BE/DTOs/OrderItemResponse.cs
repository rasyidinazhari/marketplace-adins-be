using marketplace_AdIns_BE.Models;

namespace marketplace_AdIns_BE.DTOs
{
    public class OrderItemResponse
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public long Quantity { get; set; }
        public long PriceAtPurchase { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
