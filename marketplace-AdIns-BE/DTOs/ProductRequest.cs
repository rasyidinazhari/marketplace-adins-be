using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class ProductRequest
    {
        [Required(ErrorMessage = "StoreId Wajib Diisi")]
        public long StoreId { get; set; }

        [Required(ErrorMessage = "CategoryId Wajib Diisi")]
        public long CategoryId { get; set; }

        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public decimal? ProductPrice { get; set; }
        public int? ProductStock { get; set; }

    }
}
