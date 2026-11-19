using System.ComponentModel.DataAnnotations;
namespace marketplace_AdIns_BE.Models
{
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength (150)]
        public string Email { get; set; } = string.Empty ;

        [Required]
        [MaxLength(255)]
        public string? Password { get; set; } = string.Empty;

        [Required]
        public bool isActive { get; set; } = true ;
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; } = DateTime.Now;

        public Store? Store { get; set; } = null ;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();


    }
}
