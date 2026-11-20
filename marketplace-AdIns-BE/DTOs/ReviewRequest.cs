using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class ReviewRequest
    {
        [Required(ErrorMessage = "UserId Wajib Diisi")]
        public long UserId { get; set; }

        [Required(ErrorMessage = "ProductId Wajib Diisi")]
        public long ProductId { get; set; }

        [Required(ErrorMessage ="Rating Wajib Diisi")]
        [Range(0, 5, ErrorMessage ="Rating Harus 0 - 5")]
        public long? Rating { get; set; }

        public string? Comment { get; set; }
    }
}
