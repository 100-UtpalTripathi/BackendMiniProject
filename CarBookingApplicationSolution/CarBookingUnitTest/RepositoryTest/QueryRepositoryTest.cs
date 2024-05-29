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

namespace CarBookingUnitTest.RepositoryTest
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
                var newQuery = new Query { CustomerId = 104, Subject = "New Query", Message = "New Query Message", Status = "Open", CreatedDate = DateTime.Now };

                var addedQuery = await repository.Add(newQuery);
                // Act
                var query = await repository.GetByKey(addedQuery.Id);

                // Assert
                Assert.IsNotNull(query);
                Assert.AreEqual(2, query.Id);
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
                Assert.AreEqual(0, queries.Count()); // Assuming 2 queries were seeded in the database
            }
        }

        [Test]
        public async Task UpdateQuery_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new QueryRepository(context);
                var query = new Query { CustomerId = 1, Subject = "Updated Subject", Message = "Updated Message", Status = "Closed", CreatedDate = DateTime.Now };

                var addedQuery = await repository.Add(query);
                addedQuery.Subject = "New Subject";
                // Act
                var updatedQuery = await repository.Update(addedQuery);

                // Assert
                Assert.IsNotNull(updatedQuery);
                Assert.AreEqual(addedQuery.Id, updatedQuery.Id);
                Assert.AreEqual(addedQuery.Subject, updatedQuery.Subject);
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
