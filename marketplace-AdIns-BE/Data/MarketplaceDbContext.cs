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

       /* public DbSet<Movie> Movies { get; set; }
        public DbSet<Theater> Theaters { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .Property(b => b.Status)
                .HasConversion<string>();

            // Relasi one-to-many
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.Schedules)
                .HasForeignKey(s => s.MovieId);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Theater)
                .WithMany(t => t.Schedules)
                .HasForeignKey(s => s.TheaterId);

            modelBuilder.Entity<Seat>()
                .HasOne(se => se.Schedule)
                .WithMany(s => s.Seats)
                .HasForeignKey(se => se.ScheduleId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Schedule)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relasi many-to-many lewat pivot BookingSeat
            modelBuilder.Entity<BookingSeat>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingSeats)
                .HasForeignKey(bs => bs.BookingId);

            modelBuilder.Entity<BookingSeat>()
                .HasOne(bs => bs.Seat)
                .WithMany()
                .HasForeignKey(bs => bs.SeatId);
        }*/
    }
}
