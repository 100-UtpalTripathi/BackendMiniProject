using CarBookingApplication.Contexts;
using CarBookingApplication.Exceptions.City;
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
    public class CityRepositoryTest
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
                context.Cities.AddRange(
                    new City { Id = 1, Name = "City1", State = "State1", Country = "Country1" },
                    new City { Id = 2, Name = "City2", State = "State2", Country = "Country2" }
                );
                context.SaveChanges();
            }
        }

        [Test]
        public async Task AddCityAsync_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CityRepository(context);
                var city = new City { Id = 3, Name = "City3", State = "State3", Country = "Country3" };

                // Act
                var addedCity = await repository.Add(city);

                // Assert
                Assert.IsNotNull(addedCity);
                Assert.AreEqual(city.Id, addedCity.Id);
            }
        }

        [Test]
        public async Task DeleteCityByKey_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CityRepository(context);

                // Add a city to the database
                var city = new City { Id = 4, Name = "City4", State = "State4", Country = "Country4" };
                await context.Cities.AddAsync(city);
                await context.SaveChangesAsync();

                // Act
                var deletedCity = await repository.DeleteByKey(4);

                // Assert
                Assert.IsNotNull(deletedCity);
                Assert.AreEqual(city.Id, deletedCity.Id);
            }
        }

        [Test]
        public async Task DeleteCityByKey_CityNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CityRepository(context);

                // Act & Assert
                var ex = Assert.ThrowsAsync<NoSuchCityFoundException>(() => repository.DeleteByKey(100));
                Assert.AreEqual("No such city found", ex.Message);
            }
        }

        [Test]
        public async Task GetCityByKey_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CityRepository(context);

                // Act
                var city = await repository.GetByKey(1);

                // Assert
                Assert.IsNotNull(city);
                Assert.AreEqual(1, city.Id);
            }
        }

        [Test]
        public async Task GetCityByKey_CityNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CityRepository(context);

                // Act & Assert
                var city = await repository.GetByKey(100); // Assuming ID 100 does not exist
                Assert.IsNull(city);
            }
        }

        [Test]
        public async Task GetAllCities_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CityRepository(context);

                // Act
                var cities = await repository.Get();

                // Assert
                Assert.IsNotNull(cities);
                Assert.AreEqual(2, cities.Count());
            }
        }

        [Test]
        public async Task UpdateCity_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CityRepository(context);
                var city = new City { Id = 1, Name = "UpdatedCity", State = "UpdatedState", Country = "UpdatedCountry" };

                // Act
                var updatedCity = await repository.Update(city);

                // Get the city again to verify the update
                var retrievedCity = await repository.GetByKey(city.Id);

                // Assert
                Assert.IsNotNull(updatedCity);
                Assert.AreEqual(city.Id, updatedCity.Id);
                Assert.AreEqual(city.Name, updatedCity.Name);
                Assert.AreEqual(city.State, updatedCity.State);
                Assert.AreEqual(city.Country, updatedCity.Country);

                // Additional assertion to ensure the city was updated in the database
                Assert.IsNotNull(retrievedCity);
                Assert.AreEqual(city.Name, retrievedCity.Name);
                Assert.AreEqual(city.State, retrievedCity.State);
                Assert.AreEqual(city.Country, retrievedCity.Country);
            }
        }


        [Test]
        public async Task UpdateCity_CityNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CityRepository(context);
                var city = new City { Id = 100, Name = "UpdatedCity", State = "UpdatedState", Country = "UpdatedCountry" };

                // Act & Assert
                var ex = Assert.ThrowsAsync<NoSuchCityFoundException>(() => repository.Update(city));
                Assert.AreEqual("No such city found", ex.Message);
            }
        }
    }
}
