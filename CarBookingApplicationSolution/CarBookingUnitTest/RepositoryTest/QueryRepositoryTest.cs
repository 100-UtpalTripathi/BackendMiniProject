using CarBookingApplication.Contexts;
using CarBookingApplication.Exceptions.Query;
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
    public class QueryRepositoryTest
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
                context.Queries.AddRange(
                    new Query { Id = 1, CustomerId = 1, Subject = "Test Subject 1", Message = "Test Message 1", Status = "Open", CreatedDate = DateTime.Now },
                    new Query { Id = 2, CustomerId = 2, Subject = "Test Subject 2", Message = "Test Message 2", Status = "Closed", CreatedDate = DateTime.Now }
                );
                context.SaveChanges();
            }
        }

        [Test]
        public async Task AddQueryAsync_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new QueryRepository(context);
                var query = new Query { CustomerId = 3, Subject = "New Query", Message = "New Query Message", Status = "Open", CreatedDate = DateTime.Now };

                // Act
                var addedQuery = await repository.Add(query);

                // Assert
                Assert.IsNotNull(addedQuery);
                Assert.AreEqual(query.CustomerId, addedQuery.CustomerId);
            }
        }

        [Test]
        public async Task DeleteQueryByKey_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new QueryRepository(context);

                // Act
                var deletedQuery = await repository.DeleteByKey(1);

                // Assert
                Assert.IsNotNull(deletedQuery);
                Assert.AreEqual(1, deletedQuery.Id);
            }
        }

        [Test]
        public async Task DeleteQueryByKey_QueryNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new QueryRepository(context);

                // Act & Assert
                var ex = Assert.ThrowsAsync<NoSuchQueryFoundException>(() => repository.DeleteByKey(100));
                Assert.AreEqual("No such query found", ex.Message);
            }
        }

        [Test]
        public async Task GetQueryByKey_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new QueryRepository(context);

                // Act
                var query = await repository.GetByKey(1);

                // Assert
                Assert.IsNotNull(query);
                Assert.AreEqual(1, query.Id);
            }
        }

        [Test]
        public async Task GetQueryByKey_QueryNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new QueryRepository(context);

                // Act
                var query = await repository.GetByKey(100); // Assuming ID 100 does not exist
                Assert.IsNull(query);
            }
        }

        [Test]
        public async Task GetQueries_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new QueryRepository(context);

                // Act
                var queries = await repository.Get();

                // Assert
                Assert.IsNotNull(queries);
                Assert.AreEqual(2, queries.Count()); // Assuming 2 queries were seeded in the database
            }
        }

        [Test]
        public async Task UpdateQuery_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new QueryRepository(context);
                var query = new Query { Id = 1, CustomerId = 1, Subject = "Updated Subject", Message = "Updated Message", Status = "Closed", CreatedDate = DateTime.Now };

                // Act
                var updatedQuery = await repository.Update(query);

                // Assert
                Assert.IsNotNull(updatedQuery);
                Assert.AreEqual(query.Id, updatedQuery.Id);
                Assert.AreEqual(query.Subject, updatedQuery.Subject);
            }
        }

        [Test]
        public async Task UpdateQuery_QueryNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new QueryRepository(context);
                var invalidQuery = new Query { Id = 100, CustomerId = 100, Subject = "Invalid Subject", Message = "Invalid Message", Status = "Invalid", CreatedDate = DateTime.Now };

                // Act & Assert
                var ex = Assert.ThrowsAsync<NoSuchQueryFoundException>(() => repository.Update(invalidQuery));
                Assert.AreEqual("No such query found", ex.Message);
            }
        }
    }
}
