using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using CarBookingApplication.Models.DTOs.UserDTOs;
using CarBookingApplication.Exceptions.Customer;
using CarBookingApplication.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarBookingApplication.Models.DTOs.CarRatingDTOs;
using CarBookingApplication.Exceptions.Booking;

namespace CarBookingUnitTest.ServicesTest
{
    [TestFixture]
    public class CustomerServiceTest
    {
        private Mock<IRepository<int, Customer>> _mockCustomerRepository;
        private Mock<IRepository<int, Booking>> _mockBookingRepository;
        private Mock<IRepository<int, Car>> _mockCarRepository;
        private Mock<IRepository<int, CarRating>> _mockCarRatingRepository;
        private CustomerService _customerService;

        [SetUp]
        public void Setup()
        {
            _mockCustomerRepository = new Mock<IRepository<int, Customer>>();
            _mockBookingRepository = new Mock<IRepository<int, Booking>>();
            _mockCarRepository = new Mock<IRepository<int, Car>>();
            _mockCarRatingRepository = new Mock<IRepository<int, CarRating>>();
            _customerService = new CustomerService(_mockCustomerRepository.Object, _mockBookingRepository.Object, _mockCarRepository.Object, _mockCarRatingRepository.Object);
        }

        [Test]
        public async Task GetCustomerProfileAsync_CustomerExists_ReturnsCustomer()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                Name = "John Doe",
                DateOfBirth = new DateTime(1990, 1, 1),
                Phone = "1234567890",
                Email = "john@example.com",
                Role = "Customer"
            };
            _mockCustomerRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync(customer);

            // Act
            var result = await _customerService.GetCustomerProfileAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John Doe", result.Name);
            Assert.AreEqual(new DateTime(1990, 1, 1), result.DateOfBirth);
            Assert.AreEqual("1234567890", result.Phone);
            Assert.AreEqual("john@example.com", result.Email);
            Assert.AreEqual("Customer", result.Role);
        }

        [Test]
        public void GetCustomerProfileAsync_CustomerDoesNotExist_ThrowsException()
        {
            // Arrange
            _mockCustomerRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync((Customer)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchCustomerFoundException>(() => _customerService.GetCustomerProfileAsync(1));
        }

        [Test]
        public async Task UpdateCustomerProfileAsync_CustomerExists_UpdatesCustomer()
        {
            // Arrange
            var existingCustomer = new Customer
            {
                Id = 1,
                Name = "John Doe",
                DateOfBirth = new DateTime(1990, 1, 1),
                Phone = "1234567890",
                Email = "john@example.com",
                Role = "Customer"
            };
            _mockCustomerRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync(existingCustomer);

            var updatedCustomerDTO = new CustomerUserDTO
            {
                Name = "John Smith",
                DateOfBirth = new DateTime(1991, 1, 1),
                Phone = "0987654321",
                Email = "johnsmith@example.com",
                Role = "Customer"
            };

            // Act
            var result = await _customerService.UpdateCustomerProfileAsync(1, updatedCustomerDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John Smith", result.Name);
            Assert.AreEqual(new DateTime(1991, 1, 1), result.DateOfBirth);
            Assert.AreEqual("0987654321", result.Phone);
            Assert.AreEqual("johnsmith@example.com", result.Email);
            Assert.AreEqual("Customer", result.Role);

            _mockCustomerRepository.Verify(repo => repo.Update(It.IsAny<Customer>()), Times.Once);
        }

        [Test]
        public void UpdateCustomerProfileAsync_CustomerDoesNotExist_ThrowsException()
        {
            // Arrange
            _mockCustomerRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync((Customer)null);

            var updatedCustomerDTO = new CustomerUserDTO
            {
                Name = "John Smith",
                DateOfBirth = new DateTime(1991, 1, 1),
                Phone = "0987654321",
                Email = "johnsmith@example.com",
                Role = "Customer"
            };

            // Act & Assert
            Assert.ThrowsAsync<NoSuchCustomerFoundException>(() => _customerService.UpdateCustomerProfileAsync(1, updatedCustomerDTO));
        }

        [Test]
        public async Task GetCustomerBookingsAsync_CustomerExists_ReturnsBookings()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                Bookings = new List<Booking>
                {
                    new Booking { Id = 1, CustomerId = 1, CarId = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) },
                    new Booking { Id = 2, CustomerId = 1, CarId = 2, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(2) }
                }
            };
            _mockCustomerRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync(customer);

            // Act
            var result = await _customerService.GetCustomerBookingsAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetCustomerBookingsAsync_CustomerDoesNotExist_ThrowsException()
        {
            // Arrange
            _mockCustomerRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync((Customer)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchCustomerFoundException>(() => _customerService.GetCustomerBookingsAsync(1));
        }

        [Test]
        public async Task AddRatingAsync_ValidRequest_Success()
        {
            // Arrange
            var customerId = 1;
            var carId = 1;
            var bookingId = 1;
            var carRatingDTO = new CarRatingDTO { BookingId = bookingId, CarId = carId, Rating = 5, Review = "Excellent service!" };

            var customer = new Customer { Id = customerId, Bookings = new List<Booking>() };
            var booking = new Booking { Id = bookingId, CustomerId = customerId, CarId = carId, StartDate = DateTime.Now.AddDays(-1), Status = "Confirmed" };
            var car = new Car { Id = carId, Ratings = new List<CarRating>() };

            customer.Bookings.Add(booking);

            _mockCustomerRepository.Setup(repo => repo.GetByKey(customerId)).ReturnsAsync(customer);
            _mockBookingRepository.Setup(repo => repo.GetByKey(bookingId)).ReturnsAsync(booking);
            _mockCarRepository.Setup(repo => repo.GetByKey(carId)).ReturnsAsync(car);

            // Act
            var result = await _customerService.AddRatingAsync(customerId, carRatingDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(carRatingDTO.CarId, result.CarId);
            Assert.AreEqual(customerId, result.CustomerId);
            Assert.AreEqual(carRatingDTO.Rating, result.Rating);
            Assert.AreEqual(carRatingDTO.Review, result.Review);
            Assert.IsNotNull(result.CreatedDate);

            // Verify that the rating was added to the car
            Assert.AreEqual(1, car.Ratings.Count);
            Assert.AreEqual(carRatingDTO.Rating, car.Ratings.First().Rating);

            // Verify the average rating calculation
            Assert.AreEqual(carRatingDTO.Rating, car.AverageRating);

            // Verify that the repositories were updated
            _mockCarRatingRepository.Verify(repo => repo.Add(It.IsAny<CarRating>()), Times.Once);
            _mockCarRepository.Verify(repo => repo.Update(It.IsAny<Car>()), Times.Once);
        }




        [Test]
        public void AddRatingAsync_CustomerNotFound_ThrowsException()
        {
            // Arrange
            _mockCustomerRepository.Setup(repo => repo.GetByKey(It.IsAny<int>())).ReturnsAsync((Customer)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchCustomerFoundException>(() => _customerService.AddRatingAsync(1, new CarRatingDTO()));
        }

        [Test]
        public void AddRatingAsync_BookingNotFound_ThrowsException()
        {
            

            // Arrange
            var customerId = 1;
            _mockCustomerRepository.Setup(repo => repo.GetByKey(customerId)).ReturnsAsync(new Customer { Bookings = new List<Booking>() }); // Ensure customer.Bookings is initialized with an empty list
            _mockBookingRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync((Booking)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchBookingFoundException>(() => _customerService.AddRatingAsync(customerId, new CarRatingDTO()));


            // Act & Assert
            Assert.ThrowsAsync<NoSuchBookingFoundException>(() => _customerService.AddRatingAsync(customerId, new CarRatingDTO()));

        }
    }
}
