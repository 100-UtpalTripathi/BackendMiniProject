using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using CarBookingApplication.Models.DTOs.CarDTOs;
using CarBookingApplication.Services;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarBookingUnitTest.ServicesTest
{
    public class CarServiceTest
    {
        private Mock<IRepository<int, Car>> _mockCarRepository;
        private CarService _carService;

        [SetUp]
        public void Setup()
        {
            _mockCarRepository = new Mock<IRepository<int, Car>>();
            _carService = new CarService(_mockCarRepository.Object);
        }

        [Test]
        public async Task AddCarAsync_Success()
        {
            // Arrange
            var carDto = new CarDTO
            {
                Make = "Make",
                Model = "Model",
                Year = 2023,
                CityId = 1,
                Status = "Available",
                Transmission = "Manual",
                NumberOfSeats = 5,
                Category = "Medium"
            };

            // Act
            var result = await _carService.AddCarAsync(carDto);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Car added successfully.", result.Message);
            _mockCarRepository.Verify(r => r.Add(It.IsAny<Car>()), Times.Once);
        }

        [Test]
        public async Task EditCarAsync_Success()
        {
            // Arrange
            var car = new Car { Id = 1, Make = "Make", Model = "Model", Year = 2023, CityId = 1, Status = "Available", Transmission = "Manual", NumberOfSeats = 5, Category = "Medium" };
            _mockCarRepository.Setup(r => r.GetByKey(It.IsAny<int>())).ReturnsAsync(car);

            var carDto = new CarDTO
            {
                Make = "UpdatedMake",
                Model = "UpdatedModel",
                Year = 2024,
                CityId = 2,
                Status = "Booked",
                Transmission = "Automatic",
                NumberOfSeats = 7,
                Category = "Large"
            };

            // Act
            var result = await _carService.EditCarAsync(1, carDto);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Car updated successfully.", result.Message);
            _mockCarRepository.Verify(r => r.Update(It.IsAny<Car>()), Times.Once);
        }

        [Test]
        public async Task EditCarAsync_CarNotFound()
        {
            // Arrange
            _mockCarRepository.Setup(r => r.GetByKey(It.IsAny<int>())).ReturnsAsync((Car)null);

            var carDto = new CarDTO
            {
                Make = "UpdatedMake",
                Model = "UpdatedModel",
                Year = 2024,
                CityId = 2,
                Status = "Booked",
                Transmission = "Automatic",
                NumberOfSeats = 7,
                Category = "Large"
            };

            // Act
            var result = await _carService.EditCarAsync(1, carDto);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Car not found.", result.Message);
            _mockCarRepository.Verify(r => r.Update(It.IsAny<Car>()), Times.Never);
        }

        [Test]
        public async Task DeleteCarAsync_Success()
        {
            // Arrange
            var car = new Car { Id = 1, Make = "Make", Model = "Model", Year = 2023, CityId = 1, Status = "Available", Transmission = "Manual", NumberOfSeats = 5, Category = "Medium" };
            _mockCarRepository.Setup(r => r.GetByKey(It.IsAny<int>())).ReturnsAsync(car);

            // Act
            var result = await _carService.DeleteCarAsync(1);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Car deleted successfully.", result.Message);
            _mockCarRepository.Verify(r => r.DeleteByKey(1), Times.Once);
        }

        [Test]
        public async Task DeleteCarAsync_CarNotFound()
        {
            // Arrange
            _mockCarRepository.Setup(r => r.GetByKey(It.IsAny<int>())).ReturnsAsync((Car)null);

            // Act
            var result = await _carService.DeleteCarAsync(1);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Car not found.", result.Message);
            _mockCarRepository.Verify(r => r.DeleteByKey(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task GetAllCarsAsync_Success()
        {
            // Arrange
            var cars = new List<Car>
            {
                new Car { Id = 1, Make = "Make1", Model = "Model1", Year = 2023, CityId = 1, Status = "Available", Transmission = "Manual", NumberOfSeats = 5, Category = "Medium" },
                new Car { Id = 2, Make = "Make2", Model = "Model2", Year = 2024, CityId = 2, Status = "Booked", Transmission = "Automatic", NumberOfSeats = 7, Category = "Large" }
            };
            _mockCarRepository.Setup(r => r.Get()).ReturnsAsync(cars);

            // Act
            var result = await _carService.GetAllCarsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }
    }
}
