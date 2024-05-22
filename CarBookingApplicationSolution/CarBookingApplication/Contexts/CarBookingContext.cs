using Microsoft.EntityFrameworkCore;
using CarBookingApplication.Models;

namespace CarBookingApplication.Contexts
{
    public class CarBookingContext : DbContext
    {
        public CarBookingContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Query> Queries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // City - Car (One-to-Many)
            modelBuilder.Entity<Car>()
                .HasOne(car => car.City)
                .WithMany(city => city.Cars)
                .HasForeignKey(car => car.CityId);

            // Customer - Booking (One-to-Many)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CustomerId);

            // Car - Booking (One-to-Many)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Car)
                .WithMany(car => car.Bookings)
                .HasForeignKey(b => b.CarId);

            // Customer - Query (One-to-Many)
            modelBuilder.Entity<Query>()
                .HasOne(q => q.Customer)
                .WithMany(c => c.Queries)
                .HasForeignKey(q => q.CustomerId);

            
        }
    }
}
