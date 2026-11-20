namespace marketplace_AdIns_BE.DTOs
{
    public class VoucherResponse
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public int DiscountPercentage { get; set; }
        public int MinPurchaseAmount { get; set; }
        public int MaxDiscountAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
