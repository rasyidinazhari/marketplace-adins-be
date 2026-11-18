using System.ComponentModel.DataAnnotations.Schema;

namespace marketplace_AdIns_BE.Models
{
    public class UserRole
    {
        public long Id { get; set; }

        [ForeignKey(nameof(User))]
        public long UserId { get; set; }
        public User? User { get; set; }

        [ForeignKey(nameof(Role))]
        public long RoleId { get; set; }
        public Role? Role { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set;} = DateTime.Now;

    }
}
