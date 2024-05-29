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

namespace CarBookingUnitTest.RepositoryTest
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

                var city = new City { Id = 24, Name = "City1", State = "State1", Country = "Country1" };

                await context.Cities.AddAsync(city);
                await context.SaveChangesAsync();


                // Act
                var addedCity = await repository.GetByKey(24);

                // Assert
                Assert.IsNotNull(city);
                Assert.AreEqual(24, city.Id);
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
                var city = await repository.GetByKey(105); // Assuming ID 105 does not exist
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

                var city = new City { Id = 100, Name = "City100", State = "State100", Country = "Country100" };
                var city1 = new City { Id = 101, Name = "City101", State = "State101", Country = "Country101" };

                await context.Cities.AddAsync(city);
                await context.Cities.AddAsync(city1);
                await context.SaveChangesAsync();

                // Act
                var cities = await repository.Get();

                // Assert
                Assert.IsNotNull(cities);
                Assert.AreEqual(3, cities.Count());
            }
        }

        [Test]
        public async Task UpdateCity_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CityRepository(context);
                var city = new City { Name = "New12City", State = "UpdatedState", Country = "UpdatedCountry" };
                var addedCity = await repository.Add(city);

                addedCity.Name = "UpdatedCity";
                // Act
                var updatedCity = await repository.Update(city);

                // Get the city again to verify the update
                var retrievedCity = await repository.GetByKey(city.Id);

                // Assert
                Assert.IsNotNull(updatedCity);
                Assert.AreEqual(addedCity.Id, updatedCity.Id);
                Assert.AreEqual(addedCity.Name, updatedCity.Name);
                Assert.AreEqual(addedCity.State, updatedCity.State);
                Assert.AreEqual(addedCity.Country, updatedCity.Country);

                // Additional assertion to ensure the city was updated in the database
                Assert.IsNotNull(retrievedCity);
                Assert.AreEqual(addedCity.Name, retrievedCity.Name);
                Assert.AreEqual(addedCity.State, retrievedCity.State);
                Assert.AreEqual(addedCity.Country, retrievedCity.Country);
            }
        }


        [Test]
        public async Task UpdateCity_CityNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CityRepository(context);

                var city = new City { Name = "UpdatedCity", State = "UpdatedState", Country = "UpdatedCountry" };
                var retrievedCity = await repository.Add(city);

                await repository.DeleteByKey(retrievedCity.Id);



                // Act & Assert
                var ex = Assert.ThrowsAsync<NoSuchCityFoundException>(() => repository.Update(retrievedCity));
                Assert.AreEqual("No such city found", ex.Message);
            }
        }
    }
}
