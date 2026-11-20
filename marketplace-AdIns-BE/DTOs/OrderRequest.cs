using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class OrderRequest : CheckoutRequest
    {
        [Range(1, long.MaxValue, ErrorMessage = "StoreId harus lebih dari 0")]
        public long StoreId { get; set; }

        [Range(1, long.MaxValue)]
        public long? VoucherId { get; set; }
    }
}
