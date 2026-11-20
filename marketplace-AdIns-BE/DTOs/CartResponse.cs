namespace marketplace_AdIns_BE.DTOs
{
    public class CartResponse
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public long ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public long StoreId { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
