using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace marketplace_AdIns_BE.DTOs
{
    public class ProductRequest
    {
        [Required(ErrorMessage = "StoreId Wajib Diisi")]
        public long StoreId { get; set; }

        public List<long>? CategoryIds { get; set; }

        [MinLength(5, ErrorMessage = "ProductName Minimal 5 Karakter")]
        public string? ProductName { get; set; }

        [MinLength(25, ErrorMessage = "ProductDescription Minimal 25 Karakter")]
        public string? ProductDescription { get; set; }

        [Range(typeof(decimal), "1000", "9999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "Harga produk minimal Rp 1.000 dan maksimal Rp 9.999.999.999,99.")]
        public decimal? ProductPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "ProductStock Minimal 0")]
        public int? ProductStock { get; set; }

        private int? _expectedStock;

        [Range(0, int.MaxValue)]
        public int? ExpectedStock
        {
            get => _expectedStock;
            set
            {
                _expectedStock = value;
                HasExpectedStock = true;
            }
        }

        [JsonIgnore]
        public bool HasExpectedStock { get; private set; }
    }
}
