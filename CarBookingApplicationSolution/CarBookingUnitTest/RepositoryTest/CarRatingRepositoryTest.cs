using CarBookingApplication.Contexts;
using CarBookingApplication.Exceptions.CarRating;
using CarBookingApplication.Models;
using CarBookingApplication.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CarBookingUnitTest.RepositoryTest
{
    public class CarRatingRepositoryTest
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
        public async Task AddCarRatingAsync_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CarRatingRepository(context);
                var carRating = new CarRating { CarId = 1, CustomerId = 1, Rating = 5, Review = "Superb"};

                // Act
                var addedCarRating = await repository.Add(carRating);

                // Assert
                Assert.IsNotNull(addedCarRating);
                Assert.AreEqual(carRating.CarId, addedCarRating.CarId);
                Assert.AreEqual(carRating.CustomerId, addedCarRating.CustomerId);
                Assert.AreEqual(carRating.Rating, addedCarRating.Rating);
               
            }
        }

        [Test]
        public async Task DeleteCarRatingByKey_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CarRatingRepository(context);
                var carRating = new CarRating { CarId = 1, CustomerId = 1, Rating = 5 , Review = "Superb" };
                var addedCarRating = await repository.Add(carRating);

                // Act
                var deletedCarRating = await repository.DeleteByKey(addedCarRating.Id);

                // Assert
                Assert.IsNotNull(deletedCarRating);
                Assert.AreEqual(addedCarRating.Id, deletedCarRating.Id);
            }
        }

        [Test]
        public async Task DeleteCarRatingByKey_CarRatingNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CarRatingRepository(context);

                // Act & Assert
                var ex = Assert.ThrowsAsync<NoSuchCarRatingFoundException>(() => repository.DeleteByKey(100));
                Assert.AreEqual("No such car rating found.", ex.Message);
            }
        }

        [Test]
        public async Task GetCarRatingByKey_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CarRatingRepository(context);
                var carRating = new CarRating { CarId = 1, CustomerId = 1, Rating = 5 , Review = "Superb" };
                var addedCarRating = await repository.Add(carRating);

                // Act
                var retrievedCarRating = await repository.GetByKey(addedCarRating.Id);

                // Assert
                Assert.IsNotNull(retrievedCarRating);
                Assert.AreEqual(addedCarRating.Id, retrievedCarRating.Id);
            }
        }

        [Test]
        public async Task GetCarRatingByKey_CarRatingNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CarRatingRepository(context);

                // Act
                var carRating = await repository.GetByKey(100); // Assuming ID 100 does not exist
                Assert.IsNull(carRating);
            }
        }

        [Test]
        public async Task GetCarRatings_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CarRatingRepository(context);
                var carRating = new CarRating { CarId = 1, CustomerId = 1, Rating = 5 , Review = "Superb" };
                await repository.Add(carRating);

                var carRating2 = new CarRating { CarId = 2, CustomerId = 2, Rating = 4 , Review = "Superb" };
                await repository.Add(carRating2);

                // Act
                var carRatings = await repository.Get();

                // Assert
                Assert.IsNotNull(carRatings);
                Assert.AreEqual(4, carRatings.Count()); // Assuming 2 car ratings were seeded in the database
            }
        }

        [Test]
        public async Task UpdateCarRating_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CarRatingRepository(context);
                var carRating = new CarRating { Id = 1, CarId = 1, CustomerId = 1, Rating = 5 };

                // Act
                var updatedCarRating = await repository.Update(carRating);

                // Assert
                Assert.IsNotNull(updatedCarRating);
                Assert.AreEqual(carRating.CarId, updatedCarRating.CarId);
                Assert.AreEqual(carRating.CustomerId, updatedCarRating.CustomerId);
                Assert.AreEqual(carRating.Rating, updatedCarRating.Rating);
                // Add more assertions for other properties as needed
            }
        }

        [Test]
        public async Task UpdateCarRating_CarRatingNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CarRatingRepository(context);
                var invalidCarRating = new CarRating { Id = 100, CarId = 1, CustomerId = 1, Rating = 5 , Review = "Superb" };

                // Act & Assert
                var ex = Assert.ThrowsAsync<NoSuchCarRatingFoundException>(() => repository.Update(invalidCarRating));
                Assert.AreEqual("No such car rating found.", ex.Message);
            }
        }
    }
}
