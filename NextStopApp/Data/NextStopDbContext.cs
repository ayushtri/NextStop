using NextStopApp.Models;
using Microsoft.EntityFrameworkCore;
using NextStopApp.DTOs;

namespace NextStopApp.Data
{
    public class NextStopDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<BusOperator> BusOperators { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Models.Route> Routes { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<AdminAction> AdminActions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Notification> Notifications { get; set; }



        public NextStopDbContext() { }
        public NextStopDbContext(DbContextOptions<NextStopDbContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var configSection = configBuilder.GetSection("ConnectionStrings");
            var conStr = configSection["ConStr"] ?? null;
            optionsBuilder.UseSqlServer(conStr);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasQueryFilter(u => u.IsActive);

            // User Entity
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("passenger");

            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.AdminActions)
                .WithOne(a => a.Admin)
                .HasForeignKey(a => a.AdminId)
                .OnDelete(DeleteBehavior.SetNull);


            // BusOperator Entity
            modelBuilder.Entity<BusOperator>()
                .HasKey(o => o.OperatorId);

            modelBuilder.Entity<BusOperator>()
                .HasIndex(o => o.Email)
                .IsUnique();

            modelBuilder.Entity<BusOperator>()
                .HasMany(o => o.Buses)
                .WithOne(b => b.Operator)
                .HasForeignKey(b => b.OperatorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Bus Entity
            modelBuilder.Entity<Bus>()
                .HasKey(b => b.BusId);

            modelBuilder.Entity<Bus>()
                .HasIndex(b => b.BusNumber)
                .IsUnique();

            modelBuilder.Entity<Bus>()
                .HasIndex(b => b.BusNumber)
                .IsUnique();

            modelBuilder.Entity<Bus>()
                .Property(b => b.TotalSeats)
                .IsRequired();

            modelBuilder.Entity<Bus>()
                .HasMany(b => b.Schedules)
                .WithOne(s => s.Bus)
                .HasForeignKey(s => s.BusId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Bus>()
                .HasMany(b => b.Seats)
                .WithOne(s => s.Bus)
                .HasForeignKey(s => s.BusId)
                .OnDelete(DeleteBehavior.Cascade);

            // Route Entity
            modelBuilder.Entity<Models.Route>()
                .HasKey(r => r.RouteId);

            modelBuilder.Entity<Models.Route>()
                .HasMany(r => r.Schedules)
                .WithOne(s => s.Route)
                .HasForeignKey(s => s.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Models.Route>()
                .Property(r => r.Distance)
                .HasPrecision(18, 2);

            // Schedule Entity
            modelBuilder.Entity<Schedule>()
                .HasKey(s => s.ScheduleId);

            modelBuilder.Entity<Schedule>()
                .HasMany(s => s.Bookings)
                .WithOne(b => b.Schedule)
                .HasForeignKey(b => b.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Schedule>()
                .Property(s => s.Fare)
                .HasPrecision(18, 2);

            // Booking Entity
            modelBuilder.Entity<Booking>().HasQueryFilter(b => b.User == null || b.User.IsActive);

            modelBuilder.Entity<Booking>()
                .HasKey(b => b.BookingId);

            modelBuilder.Entity<Booking>()
                .Property(b => b.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("confirmed");

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Payment)
                .WithOne(p => p.Booking)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Seats)
                .WithOne(s => s.Booking)
                .HasForeignKey(s => s.BookingId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalFare)
                .HasPrecision(18, 2);

            // Seat Entity
            modelBuilder.Entity<Seat>()
                .HasKey(s => s.SeatId);

            modelBuilder.Entity<Seat>()
                .Property(s => s.IsAvailable)
                .HasDefaultValue(true);

            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Bus)
                .WithMany(b => b.Seats)
                .HasForeignKey(s => s.BusId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Booking)
                .WithMany(b => b.Seats)
                .HasForeignKey(s => s.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            // Payment Entity
            modelBuilder.Entity<Payment>()
                .HasQueryFilter(p => p.Booking == null || p.Booking.User == null || p.Booking.User.IsActive);

            modelBuilder.Entity<Payment>()
                .HasKey(p => p.PaymentId);

            modelBuilder.Entity<Payment>()
                .Property(p => p.PaymentStatus)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            // AdminAction Entity
            modelBuilder.Entity<AdminAction>()
                .HasKey(a => a.ActionId);

            modelBuilder.Entity<AdminAction>()
                .Property(a => a.AdminId)
                .IsRequired(false);

            modelBuilder.Entity<AdminAction>()
                .HasOne(a => a.Admin)
                .WithMany(u => u.AdminActions)
                .HasForeignKey(a => a.AdminId)
                .OnDelete(DeleteBehavior.SetNull);

            //Notification
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }

    }
}