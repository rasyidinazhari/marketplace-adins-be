using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class CheckoutRequest
    {
        [Range(1, long.MaxValue, ErrorMessage = "AddressId harus lebih dari 0")]
        public long AddressId { get; set; }

        public string? VoucherCode { get; set; }
    }
}
