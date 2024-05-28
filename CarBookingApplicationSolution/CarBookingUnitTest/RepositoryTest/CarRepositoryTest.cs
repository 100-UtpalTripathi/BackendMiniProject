using CarBookingApplication.Contexts;
using CarBookingApplication.Exceptions.Car;
using CarBookingApplication.Models;
using CarBookingApplication.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarBookingUnitTest
{
    public class CarRepositoryTest
    {
        private DbContextOptions<CarBookingContext> _options;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<CarBookingContext>()
                .UseInMemoryDatabase(databaseName: "test_database")
                .Options;

            // Seed the database with test data
            using (var context = new CarBookingContext(_options))
            {
                context.Cars.AddRange(
                    new Car { Id = 1, Make = "Toyota", Model = "Corolla", Year = 2020, CityId = 1, Status = "Available", Transmission = "Automatic", NumberOfSeats = 5, Category = "Medium" },
                    new Car { Id = 2, Make = "Honda", Model = "Civic", Year = 2019, CityId = 2, Status = "Available", Transmission = "Manual", NumberOfSeats = 5, Category = "Medium" }
                );
                context.SaveChanges();
            }
        }

        [Test]
        public async Task AddCarAsync_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CarRepository(context);
                var car = new Car { Make = "Ford", Model = "Fiesta", Year = 2021, CityId = 1, Status = "Available", Transmission = "Manual", NumberOfSeats = 5, Category = "Small" };

                // Act
                var addedCar = await repository.Add(car);

                // Assert
                Assert.IsNotNull(addedCar);
                Assert.AreEqual(car.Make, addedCar.Make);
                Assert.AreEqual(car.Model, addedCar.Model);
                // Add more assertions for other properties as needed
            }
        }

        [Test]
        public async Task DeleteCarByKey_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CarRepository(context);

                // Act
                var deletedCar = await repository.DeleteByKey(1);

                // Assert
                Assert.IsNotNull(deletedCar);
                Assert.AreEqual(1, deletedCar.Id);
            }
        }

        [Test]
        public async Task DeleteCarByKey_CarNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CarRepository(context);

                // Act & Assert
                var ex = Assert.ThrowsAsync<NoSuchCarFoundException>(() => repository.DeleteByKey(100));
                Assert.AreEqual("No Car found with given ID!", ex.Message);
            }
        }

        [Test]
        public async Task GetCarByKey_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CarRepository(context);

                // Act
                var car = await repository.GetByKey(1);

                // Assert
                Assert.IsNotNull(car);
                Assert.AreEqual(1, car.Id);
            }
        }

        [Test]
        public async Task GetCarByKey_CarNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CarRepository(context);

                // Act
                var car = await repository.GetByKey(100); // Assuming ID 100 does not exist
                Assert.IsNull(car);
            }
        }

        [Test]
        public async Task GetCars_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CarRepository(context);

                // Act
                var cars = await repository.Get();

                // Assert
                Assert.IsNotNull(cars);
                Assert.AreEqual(2, cars.Count()); // Assuming 2 cars were seeded in the database
            }
        }

        [Test]
        public async Task UpdateCar_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CarRepository(context);
                var car = new Car { Id = 1, Make = "Ford", Model = "Fiesta", Year = 2021, CityId = 1, Status = "Available", Transmission = "Manual", NumberOfSeats = 5, Category = "Small" };

                // Act
                var updatedCar = await repository.Update(car);

                // Assert
                Assert.IsNotNull(updatedCar);
                Assert.AreEqual(car.Make, updatedCar.Make);
                Assert.AreEqual(car.Model, updatedCar.Model);
                // Add more assertions for other properties as needed
            }
        }

        [Test]
        public async Task UpdateCar_CarNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CarRepository(context);
                var invalidCar = new Car { Id = 100, Make = "InvalidMake", Model = "InvalidModel", Year = 2022, CityId = 1, Status = "Available", Transmission = "Manual", NumberOfSeats = 5, Category = "Small" };

                // Act & Assert
                var ex = Assert.ThrowsAsync<NoSuchCarFoundException>(() => repository.Update(invalidCar));
                Assert.AreEqual("No Car found with given ID!", ex.Message);
            }
        }
    }
}
