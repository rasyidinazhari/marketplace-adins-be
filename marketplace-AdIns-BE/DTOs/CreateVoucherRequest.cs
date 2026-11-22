using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class CreateVoucherRequest
    {
        [Required, MaxLength(100)]
        public string Code { get; set; } = string.Empty;

        [Range(1, 100)]
        public int DiscountPercentage { get; set; }

        [Range(0, int.MaxValue)]
        public int MinPurchaseAmount { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxDiscountAmount { get; set; }
    }
}
