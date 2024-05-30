using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using CarBookingApplication.Models.DTOs.CityDTOs;
using CarBookingApplication.Services;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarBookingUnitTest.ServicesTest
{
    public class CityServiceTest
    {
        private Mock<IRepository<int, City>> _mockCityRepository;
        private CityService _cityService;

        [SetUp]
        public void Setup()
        {
            _mockCityRepository = new Mock<IRepository<int, City>>();
            _cityService = new CityService(_mockCityRepository.Object);
        }

        [Test]
        public async Task AddCityAsync_Success()
        {
            // Arrange
            var cityDto = new CityDTO
            {
                Name = "CityName",
                State = "StateName",
                Country = "CountryName"
            };

            // Act
            var result = await _cityService.AddCityAsync(cityDto);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("City added successfully.", result.Message);
            _mockCityRepository.Verify(r => r.Add(It.IsAny<City>()), Times.Once);
        }

        [Test]
        public async Task EditCityAsync_Success()
        {
            // Arrange
            var city = new City { Id = 1, Name = "CityName", State = "StateName", Country = "CountryName" };
            _mockCityRepository.Setup(r => r.GetByKey(It.IsAny<int>())).ReturnsAsync(city);

            var cityDto = new CityDTO
            {
                Name = "UpdatedCityName",
                State = "UpdatedStateName",
                Country = "UpdatedCountryName"
            };

            // Act
            var result = await _cityService.EditCityAsync(1, cityDto);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("City updated successfully.", result.Message);
            _mockCityRepository.Verify(r => r.Update(It.IsAny<City>()), Times.Once);
        }

        [Test]
        public async Task EditCityAsync_CityNotFound()
        {
            // Arrange
            _mockCityRepository.Setup(r => r.GetByKey(It.IsAny<int>())).ReturnsAsync((City)null);

            var cityDto = new CityDTO
            {
                Name = "UpdatedCityName",
                State = "UpdatedStateName",
                Country = "UpdatedCountryName"
            };

            // Act
            var result = await _cityService.EditCityAsync(1, cityDto);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("City not found.", result.Message);
            _mockCityRepository.Verify(r => r.Update(It.IsAny<City>()), Times.Never);
        }

        [Test]
        public async Task DeleteCityAsync_Success()
        {
            // Arrange
            var city = new City { Id = 1, Name = "CityName", State = "StateName", Country = "CountryName" };
            _mockCityRepository.Setup(r => r.GetByKey(It.IsAny<int>())).ReturnsAsync(city);

            // Act
            var result = await _cityService.DeleteCityAsync(1);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("City deleted successfully.", result.Message);
            _mockCityRepository.Verify(r => r.DeleteByKey(1), Times.Once);
        }

        [Test]
        public async Task DeleteCityAsync_CityNotFound()
        {
            // Arrange
            _mockCityRepository.Setup(r => r.GetByKey(It.IsAny<int>())).ReturnsAsync((City)null);

            // Act
            var result = await _cityService.DeleteCityAsync(1);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("City not found.", result.Message);
            _mockCityRepository.Verify(r => r.DeleteByKey(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task GetAllCitiesAsync_Success()
        {
            // Arrange
            var cities = new List<City>
            {
                new City { Id = 1, Name = "City1", State = "State1", Country = "Country1" },
                new City { Id = 2, Name = "City2", State = "State2", Country = "Country2" }
            };
            _mockCityRepository.Setup(r => r.Get()).ReturnsAsync(cities);

            // Act
            var result = await _cityService.GetAllCitiesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }
    }
}
