using marketplace_AdIns_BE.Models;
using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class OrderRequest
    {
        public long UserId { get; set; }
        public long StoreId { get; set; }
        public long AddressId { get; set; }
        public long? VoucherId { get; set; } = null;
        public decimal? DiscountAmount { get; set; } = 0;
        public decimal? TotalBeforeDisc { get; set; } = 0;
        public decimal? TotalAfterDisc { get; set; } = 0;
        public OrderStatus? OrderStatus { get; set; }

    }
}
