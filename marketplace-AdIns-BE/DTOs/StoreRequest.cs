using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class StoreRequest
    {
        [Required(ErrorMessage = "UserId Wajib Diisi")]
        public long UserId { get; set; }

        [Required(ErrorMessage = "StoreName Wajib Diisi")]
        [MinLength(5, ErrorMessage = "StoreName minimal 5 karakter.")]
        [MaxLength(100, ErrorMessage = "StoreName maksimal 100 karakter.")]
        public string StoreName { get; set; } = string.Empty;
    }
}
