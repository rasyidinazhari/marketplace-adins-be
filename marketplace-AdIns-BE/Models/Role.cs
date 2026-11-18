using System.ComponentModel.DataAnnotations;

namespace marketplace_AdIns_BE.Models
{
    public class Role
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string? RoleName { get; set; } = string.Empty;

        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
