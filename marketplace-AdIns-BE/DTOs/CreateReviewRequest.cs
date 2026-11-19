using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class CreateReviewRequest
    {
        [Required]
        public decimal Rating { get; set; }

        public string? Comment { get; set; }
    }
}
