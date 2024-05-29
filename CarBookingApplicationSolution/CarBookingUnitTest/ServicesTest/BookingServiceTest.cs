using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarBookingApplication.Exceptions.Booking;
using CarBookingApplication.Exceptions.Car;
using CarBookingApplication.Exceptions.Customer;
using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using CarBookingApplication.Models.DTOs.BookingDTOs;
using CarBookingApplication.Services;
using Moq;
using NUnit.Framework;

namespace CarBookingUnitTest.ServicesTest
{
    public class BookingServiceTest
    {
        private Mock<IRepository<int, Booking>> _mockBookingRepository;
        private Mock<IRepository<int, Customer>> _mockCustomerRepository;
        private Mock<IRepository<int, Car>> _mockCarRepository;
        private BookingService _bookingService;

        [SetUp]
        public void Setup()
        {
            _mockBookingRepository = new Mock<IRepository<int, Booking>>();
            _mockCustomerRepository = new Mock<IRepository<int, Customer>>();
            _mockCarRepository = new Mock<IRepository<int, Car>>();
            _bookingService = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockCarRepository.Object);
        }

        [Test]
        public async Task GetAllBookingsAsync_Admin_ReturnsAllBookings()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer { Id = customerId, Role = "Admin" };
            var bookings = new List<Booking>
            {
                new Booking { Id = 1, CustomerId = 2 },
                new Booking { Id = 2, CustomerId = 3 }
            };

            _mockCustomerRepository.Setup(repo => repo.GetByKey(customerId)).ReturnsAsync(customer);
            _mockBookingRepository.Setup(repo => repo.Get()).ReturnsAsync(bookings);

            // Act
            var result = await _bookingService.GetAllBookingsAsync(customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAllBookingsAsync_Customer_ReturnsCustomerBookings()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer { Id = customerId, Role = "User" };
            var bookings = new List<Booking>
            {
                new Booking { Id = 1, CustomerId = customerId },
                new Booking { Id = 2, CustomerId = 2 }
            };

            _mockCustomerRepository.Setup(repo => repo.GetByKey(customerId)).ReturnsAsync(customer);
            _mockBookingRepository.Setup(repo => repo.Get()).ReturnsAsync(bookings);

            // Act
            var result = await _bookingService.GetAllBookingsAsync(customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(customerId, result.First().CustomerId);
        }

        [Test]
        public async Task GetBookingByIdAsync_Admin_ReturnsBooking()
        {
            // Arrange
            var customerId = 1;
            var bookingId = 1;
            var customer = new Customer { Id = customerId, Role = "Admin" };
            var booking = new Booking { Id = bookingId, CustomerId = 2 };

            _mockCustomerRepository.Setup(repo => repo.GetByKey(customerId)).ReturnsAsync(customer);
            _mockBookingRepository.Setup(repo => repo.GetByKey(bookingId)).ReturnsAsync(booking);

            // Act
            var result = await _bookingService.GetBookingByIdAsync(bookingId, customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(bookingId, result.Id);
        }

        [Test]
        public async Task GetBookingByIdAsync_Customer_ReturnsBooking()
        {
            // Arrange
            var customerId = 1;
            var bookingId = 1;
            var customer = new Customer { Id = customerId, Role = "User" };
            var booking = new Booking { Id = bookingId, CustomerId = customerId };

            _mockCustomerRepository.Setup(repo => repo.GetByKey(customerId)).ReturnsAsync(customer);
            _mockBookingRepository.Setup(repo => repo.GetByKey(bookingId)).ReturnsAsync(booking);

            // Act
            var result = await _bookingService.GetBookingByIdAsync(bookingId, customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(bookingId, result.Id);
        }

        [Test]
        public void GetBookingByIdAsync_CustomerNotAuthorized_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            int customerId = 10;
            int bookingId = 10;
            var customer = new Customer { Id = customerId, Role = "User" };
            var booking = new Booking { Id = bookingId, CustomerId = 2 };

            _mockCustomerRepository.Setup(repo => repo.GetByKey(customerId)).ReturnsAsync(customer);
            _mockBookingRepository.Setup(repo => repo.GetByKey(bookingId)).ReturnsAsync(booking);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _bookingService.GetBookingByIdAsync(bookingId, customerId));
        }

        [Test]
        public async Task CancelBookingAsync_ValidCustomer_CancelsSuccessfully()
        {
            // Arrange
            var customerId = 1;
            var bookingId = 1;
            var booking = new Booking { Id = bookingId, CustomerId = customerId, StartDate = DateTime.Now.AddDays(3), Status = "Confirmed" };

            _mockBookingRepository.Setup(repo => repo.GetByKey(bookingId)).ReturnsAsync(booking);
            _mockBookingRepository.Setup(repo => repo.GetByKey(bookingId)).ReturnsAsync(booking);



            // Act
            var result = await _bookingService.CancelBookingAsync(bookingId, customerId);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Booking cancelled successfully.", result.Message);
            Assert.AreEqual("Cancelled", booking.Status);
        }

        [Test]
        public async Task CancelBookingAsync_BookingNotFound_ReturnsFailure()
        {
            // Arrange
            var customerId = 1;
            var bookingId = 1;

            _mockBookingRepository.Setup(repo => repo.GetByKey(bookingId)).ReturnsAsync((Booking)null);

            // Act
            var result = await _bookingService.CancelBookingAsync(bookingId, customerId);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual($"Booking with ID {bookingId} not found.", result.Message);
        }

        [Test]
        public async Task CancelBookingAsync_CustomerNotAuthorized_ReturnsFailure()
        {
            // Arrange
            var customerId = 1;
            var bookingId = 1;
            var booking = new Booking { Id = bookingId, CustomerId = 2, StartDate = DateTime.Now.AddDays(3), Status = "Confirmed" };

            _mockBookingRepository.Setup(repo => repo.GetByKey(bookingId)).ReturnsAsync(booking);

            // Act
            var result = await _bookingService.CancelBookingAsync(bookingId, customerId);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("You are not authorized to cancel this booking.", result.Message);
        }

        [Test]
        public async Task CancelBookingAsync_BookingAlreadyStarted_ReturnsFailure()
        {
            // Arrange
            var customerId = 1;
            var bookingId = 1;
            var booking = new Booking { Id = bookingId, CustomerId = customerId, StartDate = DateTime.Now.AddDays(-1), Status = "Confirmed" };

            _mockBookingRepository.Setup(repo => repo.GetByKey(bookingId)).ReturnsAsync(booking);

            // Act
            var result = await _bookingService.CancelBookingAsync(bookingId, customerId);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Booking has already started. Cannot cancel.", result.Message);
        }

        [Test]
        public async Task BookCarAsync_SuccessfulBooking_ReturnsBooking()
        {
            // Arrange
            var customerId = 1;
            var carId = 1;
            var bookingRequest = new BookingDTO
            {
                CarId = carId,
                BookingDate = DateTime.Now,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(5)
            };

            var car = new Car { Id = carId, Status = "Available" };
            var customer = new Customer { Id = customerId, Bookings = new List<Booking>() };

            _mockCarRepository.Setup(repo => repo.GetByKey(carId)).ReturnsAsync(car);
            _mockCustomerRepository.Setup(repo => repo.GetByKey(customerId)).ReturnsAsync(customer);
            _mockBookingRepository.Setup(repo => repo.Add(It.IsAny<Booking>())).ReturnsAsync((Booking b) => b);
            _mockCarRepository.Setup(repo => repo.GetByKey(It.IsAny<int>())).ReturnsAsync(new Car { Id = 1, Status = "Available" });


            // Act
            var result = await _bookingService.BookCarAsync(customerId, bookingRequest);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(carId, result.CarId);
            Assert.AreEqual(customerId, result.CustomerId);
            Assert.AreEqual("Confirmed", result.Status);
            _mockBookingRepository.Verify(repo => repo.Add(It.IsAny<Booking>()), Times.Once);
            _mockCarRepository.Verify(repo => repo.Update(It.IsAny<Car>()), Times.Once);
        }


        [Test]
        public void BookCarAsync_CarNotFound_ThrowsNoSuchCarFoundException()
        {
            var customerId = 1;
            var carId = 1;


            // Create a booking request with the current date
            var bookingRequest = new BookingDTO
            {
                CarId = carId,
                BookingDate = DateTime.Now,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(5)
            };

            // Act & Assert
            // Ensure that the method under test throws a NoSuchCarFoundException when the car is not found
            Assert.ThrowsAsync<NoSuchCarFoundException>(() => _bookingService.BookCarAsync(customerId, bookingRequest));
        }


        [Test]
        public void BookCarAsync_CarNotAvailable_ThrowsCarNotAvailableForBookingException()
        {
            // Arrange
            var customerId = 1;
            var bookingRequest = new BookingDTO
            {
                CarId = 1,
                BookingDate = DateTime.Now,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(5)
            };

            var car = new Car { Id = 1, Status = "Booked" };

            _mockCarRepository.Setup(repo => repo.GetByKey(bookingRequest.CarId)).ReturnsAsync(car);

            // Act & Assert
            Assert.ThrowsAsync<CarNotAvailableForBookingException>(() => _bookingService.BookCarAsync(customerId, bookingRequest));
        }

        [Test]
        public void BookCarAsync_CustomerNotFound_ThrowsNoSuchCustomerFoundException()
        {
            // Arrange
            var customerId = 1;
            var bookingRequest = new BookingDTO
            {
                CarId = 1,
                BookingDate = DateTime.Now,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(5)
            };

            var car = new Car { Id = 1, Status = "Available" };

            _mockCarRepository.Setup(repo => repo.GetByKey(bookingRequest.CarId)).ReturnsAsync(car);
            _mockCustomerRepository.Setup(repo => repo.GetByKey(customerId)).ReturnsAsync((Customer)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchCustomerFoundException>(() => _bookingService.BookCarAsync(customerId, bookingRequest));
        }

        [Test]
        public async Task CancelBookingAsync_BookingWithin48HoursNoCancellationFee_AppliesNoCancellationFee()
        {
            // Arrange
            var customerId = 1;
            var bookingId = 1;
            var booking = new Booking { Id = bookingId, CustomerId = customerId, StartDate = DateTime.Now.AddDays(3), Status = "Confirmed" }; // Start date is 2 days from now

            _mockBookingRepository.Setup(repo => repo.GetByKey(bookingId)).ReturnsAsync(booking);

            // Act
            var result = await _bookingService.CancelBookingAsync(bookingId, customerId);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(booking.TotalAmount, booking.FinalAmount); // No cancellation fee applied, so final amount should remain the same as total amount

        }

        [Test]
        public async Task CancelBookingAsync_BookingWithin48HoursCancellationFee_AppliesCancellationFee()
        {
            // Arrange
            var customerId = 1;
            var bookingId = 1;
            var booking = new Booking { Id = bookingId, CustomerId = customerId, StartDate = DateTime.Now.AddDays(1), Status = "Confirmed" }; // Start date is 1 day from now

            _mockBookingRepository.Setup(repo => repo.GetByKey(bookingId)).ReturnsAsync(booking);

            // Act
            var result = await _bookingService.CancelBookingAsync(bookingId, customerId);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Booking cancelled successfully. Cancellation fee applied.", result.Message);
            Assert.AreEqual("Cancelled", booking.Status);
            // You may want to calculate the expected cancellation fee based on the time difference and assert it
        }


    }
}
