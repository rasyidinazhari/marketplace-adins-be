using System.ComponentModel.DataAnnotations;
namespace marketplace_AdIns_BE.Models
{
    public class Category
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

    }
}
