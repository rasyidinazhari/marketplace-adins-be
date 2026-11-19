using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class CreateProductRequest
    {
        [Required(ErrorMessage = "StoreId harus terisi")]
        public long StoreId { get; set; }

        [Required(ErrorMessage = "CategoryId harus terisi")]
        public long CategoryId { get; set; }

        [Required(ErrorMessage = "ProductName harus terisi")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ProductDescription harus terisi")]
        public string ProductDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "ProductPrice harus terisi")]
        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage = "ProductStock harus terisi")]
        public int ProductStock { get; set; }
    }
}
