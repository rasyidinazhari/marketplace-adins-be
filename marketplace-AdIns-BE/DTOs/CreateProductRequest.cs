using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class CreateProductRequest
    {
        [Range(1, long.MaxValue, ErrorMessage = "StoreId harus lebih dari 0")]
        public long StoreId { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "CategoryId harus lebih dari 0")]
        public long CategoryId { get; set; }

        [Required(ErrorMessage = "ProductName harus terisi")]
        public string ProductName { get; set; } = string.Empty;

        public string? ProductDescription { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "9999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "ProductPrice tidak valid")]
        public decimal ProductPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "ProductStock tidak boleh negatif")]
        public int ProductStock { get; set; }
    }
}
