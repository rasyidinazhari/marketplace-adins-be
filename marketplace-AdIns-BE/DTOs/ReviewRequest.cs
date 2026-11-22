using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class ReviewRequest
    {
        public long UserId { get; set; }

        public long ProductId { get; set; }

        public long? OrderId { get; set; }

        [Required(ErrorMessage = "Rating Wajib Diisi")]
        [Range(1, 5, ErrorMessage = "Rating Harus 1 - 5")]
        public long? Rating { get; set; }

        public string? Comment { get; set; }
    }
}
