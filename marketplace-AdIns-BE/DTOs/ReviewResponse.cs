using marketplace_AdIns_BE.Models;

namespace marketplace_AdIns_BE.DTOs
{
    public class ReviewResponse
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string? UserName { get; set; }
        public long ProductId { get; set; }
        public string? ProductName { get; set; }
        public long Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
