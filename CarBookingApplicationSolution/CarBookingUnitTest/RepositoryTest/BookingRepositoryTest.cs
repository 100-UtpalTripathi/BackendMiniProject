using CarBookingApplication.Contexts;
using CarBookingApplication.Exceptions.Booking;
using CarBookingApplication.Models;
using CarBookingApplication.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarBookingUnitTest.RepositoryTest
{
    public class BookingRepositoryTest
    {
        private DbContextOptions<CarBookingContext> _options;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<CarBookingContext>()
                .UseInMemoryDatabase(databaseName: "test_database")
                .Options;
        }

        [Test]
        public async Task AddBookingAsync_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new BookingRepository(context);
                var booking = new Booking
                {
                    CarId = 1,
                    CustomerId = 1,
                    BookingDate = DateTime.Now,
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(3),
                    TotalAmount = 150,
                    DiscountAmount = 0,
                    FinalAmount = 150,
                    Status = "Confirmed"
                };

                // Act
                var addedBooking = await repository.Add(booking);

                // Assert
                Assert.IsNotNull(addedBooking);
                Assert.AreEqual(booking.CarId, addedBooking.CarId);
                Assert.AreEqual(booking.CustomerId, addedBooking.CustomerId);
                // Add more assertions for other properties as needed
            }
        }

        [Test]
        public async Task DeleteBookingByKey_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new BookingRepository(context);

                var booking = new Booking
                {
                    CarId = 5,
                    CustomerId = 10,
                    BookingDate = DateTime.Now,
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(3),
                    TotalAmount = 180,
                    DiscountAmount = 0,
                    FinalAmount = 180,
                    Status = "Confirmed"
                };

                // Act
                var addedBooking = await repository.Add(booking);

                var deletedBooking = await repository.DeleteByKey(addedBooking.Id);

                // Assert
                Assert.IsNotNull(deletedBooking);
                Assert.AreEqual(addedBooking.Id, deletedBooking.Id);
            }
        }

        [Test]
        public async Task DeleteBookingByKey_BookingNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new BookingRepository(context);

                // Act & Assert
                var ex = Assert.ThrowsAsync<NoSuchBookingFoundException>(() => repository.DeleteByKey(100));
                Assert.AreEqual("No Booking found with given ID!", ex.Message);
            }
        }

        [Test]
        public async Task GetBookingByKey_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new BookingRepository(context);

                // Act
                var booking = await repository.GetByKey(1);

                // Assert
                Assert.IsNotNull(booking);
                Assert.AreEqual(1, booking.Id);
            }
        }

        [Test]
        public async Task GetBookingByKey_BookingNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new BookingRepository(context);

                // Act
                var booking = await repository.GetByKey(100); // Assuming ID 100 does not exist
                Assert.IsNull(booking);
            }
        }

        [Test]
        public async Task GetBookings_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new BookingRepository(context);
                var booking1 = new Booking
                {
                    CarId = 5,
                    CustomerId = 10,
                    BookingDate = DateTime.Now,
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(3),
                    TotalAmount = 180,
                    DiscountAmount = 0,
                    FinalAmount = 180,
                    Status = "Confirmed"
                };
                var booking2 = new Booking
                {
                    CarId = 10,
                    CustomerId = 5,
                    BookingDate = DateTime.Now,
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(3),
                    TotalAmount = 1200,
                    DiscountAmount = 0,
                    FinalAmount = 1200,
                    Status = "Confirmed"
                };

               
                await repository.Add(booking1);
                await repository.Add(booking2);

                // Act
                var bookings = await repository.Get();

                // Assert
                Assert.IsNotNull(bookings);
                Assert.AreEqual(3, bookings.Count()); // Assuming 2 bookings were seeded in the database
            }
        }

        [Test]
        public async Task UpdateBooking_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new BookingRepository(context);
                var booking = new Booking
                {
                    Id = 1,
                    CarId = 1,
                    CustomerId = 1,
                    BookingDate = DateTime.Now,
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(3),
                    TotalAmount = 150,
                    DiscountAmount = 0,
                    FinalAmount = 150,
                    Status = "Cancelled"
                };

                // Act
                var updatedBooking = await repository.Update(booking);

                // Assert
                Assert.IsNotNull(updatedBooking);
                Assert.AreEqual(booking.Status, updatedBooking.Status);
                // Add more assertions for other properties as needed
            }
        }

        [Test]
        public async Task UpdateBooking_BookingNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new BookingRepository(context);
                var invalidBooking = new Booking
                {
                    Id = 100,
                    CarId = 1,
                    CustomerId = 1,
                    BookingDate = DateTime.Now,
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(3),
                    TotalAmount = 150,
                    DiscountAmount = 0,
                    FinalAmount = 150,
                    Status = "Cancelled"
                };

                // Act & Assert
                var ex = Assert.ThrowsAsync<NoSuchBookingFoundException>(() => repository.Update(invalidBooking));
                Assert.AreEqual("No Booking found with given ID!", ex.Message);
            }
        }
    }
}
