namespace marketplace_AdIns_BE.DTOs
{
    public class OrderResponse
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public long StoreId { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public long AddressId { get; set; }
        public string RecipientName { get; set; } = string.Empty ;
        public long? VoucherId { get; set; } = null;
        public string? VoucherCode { get; set; } = string.Empty;
        public decimal? DiscountAmount { get; set; } = 0;
        public decimal? TotalBeforeDisc { get; set; } = 0;
        public decimal? TotalAfterDisc { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
