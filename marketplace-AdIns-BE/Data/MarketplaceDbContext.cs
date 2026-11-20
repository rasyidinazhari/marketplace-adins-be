using Microsoft.EntityFrameworkCore;
using marketplace_AdIns_BE.Models;
namespace marketplace_AdIns_BE.Data
{
    public class MarketplaceDbContext : DbContext
    {
        public MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options)
            : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>()
                .HasIndex(u => u.RoleName)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(u => u.CategoryName)
                .IsUnique();

            modelBuilder.Entity<Voucher>()
                .HasIndex(u => u.Code)
                .IsUnique();

            modelBuilder.Entity<Store>()
                .HasIndex(u => u.StoreName)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .Property(b => b.Status)
                .HasConversion<string>();
            

            // Relasi one-to-many
            modelBuilder.Entity<Address>()
                .HasOne(s => s.User)
                .WithMany(m => m.Addresses)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
                .HasOne(s => s.User)
                .WithMany(t => t.Carts)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<Cart>()
                .HasOne(se => se.Product)
                .WithMany(s => s.Carts)
                .HasForeignKey(se => se.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            /*modelBuilder.Entity<Product>()
                .HasOne(b => b.Category)
                .WithMany(s => s.Products)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);*/

            modelBuilder.Entity<Product>()
                .HasOne(b => b.Store)
                .WithMany(u => u.Products)
                .HasForeignKey(b => b.StoreId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(b => b.Product)
                .WithMany(u => u.OrderItems)
                .HasForeignKey(b => b.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(b => b.Order)
                .WithMany(u => u.OrderItems)
                .HasForeignKey(b => b.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(b => b.Product)
                .WithMany(u => u.Reviews)
                .HasForeignKey(b => b.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(b => b.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(b => b.Address)
                .WithMany(u => u.Orders)
                .HasForeignKey(b => b.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(b => b.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(b => b.Store)
                .WithMany(u => u.Orders)
                .HasForeignKey(b => b.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(b => b.Voucher)
                .WithMany(u => u.Orders)
                .HasForeignKey(b => b.VoucherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Store>()
                .HasOne(b => b.User)
                .WithMany(u => u.Stores)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Voucher>()
                .HasOne(b => b.Store)
                .WithMany(u => u.Vouchers)
                .HasForeignKey(b => b.StoreId)
                .OnDelete(DeleteBehavior.Cascade);

            /*// Relasi One to One
            modelBuilder.Entity<User>()
                .HasOne(b => b.Store)
                .WithOne(u => u.User)
                .HasForeignKey<User>(b => b.Id)
                .OnDelete(DeleteBehavior.Restrict);*/


            // Relasi many-to-many lewat pivot UserRole
            modelBuilder.Entity<UserRole>()
                .HasOne(bs => bs.User)
                .WithMany(b => b.UserRoles)
                .HasForeignKey(bs => bs.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(bs => bs.Role)
                .WithMany()
                .HasForeignKey(bs => bs.RoleId);


            modelBuilder.Entity<ProductCategory>()
                .HasOne(bs => bs.Product)
                .WithMany(b => b.ProductCategories)
                .HasForeignKey(bs => bs.ProductId);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(bs => bs.Category)
                .WithMany()
                .HasForeignKey(bs => bs.CategoryId);
        }
    }
}
