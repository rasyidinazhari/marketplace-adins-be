using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.DTOs
{
    public class CreateReviewRequest
    {
        [Range(typeof(decimal), "1", "5", ErrorMessage = "Rating harus di antara 1 sampai 5")]
        public decimal Rating { get; set; }

        public string? Comment { get; set; }
    }
}
