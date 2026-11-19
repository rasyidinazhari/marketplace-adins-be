using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class ProductRequest
    {
        [Required(ErrorMessage = "StoreId Wajib Diisi")]
        public long StoreId { get; set; }

        [Required(ErrorMessage = "CategoryId Wajib Diisi")]
        public long CategoryId { get; set; }

        [MinLength(5, ErrorMessage = "ProductName Minimal 5 Karakter")]
        public string? ProductName { get; set; }

        [MinLength(25, ErrorMessage = "ProductDescription Minimal 25 Karakter")]
        public string? ProductDescription { get; set; }

        [Range(1000, double.MaxValue, ErrorMessage = "ProductPrice Minimal 1000")]
        public decimal? ProductPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "ProductStock Minimal 0")]
        public int? ProductStock { get; set; }

    }
}
