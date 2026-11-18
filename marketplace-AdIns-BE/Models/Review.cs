using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace marketplace_AdIns_BE.Models
{
    public class Review
    {
        [Key]
        public long Id { get; set; }    

        [ForeignKey(nameof(User))]
        public long UserId { get; set; }
        public User? User { get; set; }

        public long ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        [Column(TypeName = "decimal(2,1)")]
        [Range(0, 5, ErrorMessage = "Rating Minimal 0 & Maksimal 5")]
        public long Rating { get; set; }

        [Required]
        public string? comment { get; set; } = string.Empty;
    }
}
