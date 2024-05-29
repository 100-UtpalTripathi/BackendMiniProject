using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarBookingApplication.Exceptions.Customer;
using CarBookingApplication.Exceptions.Query;
using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using CarBookingApplication.Models.DTOs.QueryDTOs;
using CarBookingApplication.Services;
using Moq;
using NUnit.Framework;

namespace CarBookingUnitTest.ServicesTest
{
    [TestFixture]
    public class QueryServiceTest
    {
        private Mock<IRepository<int, Query>> _mockQueryRepository;
        private Mock<IRepository<int, Customer>> _mockCustomerRepository;
        private QueryService _queryService;

        [SetUp]
        public void Setup()
        {
            _mockQueryRepository = new Mock<IRepository<int, Query>>();
            _mockCustomerRepository = new Mock<IRepository<int, Customer>>();
            _queryService = new QueryService(_mockQueryRepository.Object, _mockCustomerRepository.Object);
        }

        [Test]
        public async Task SubmitQueryAsync_ValidCustomer_ReturnsQuery()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer { Id = customerId, Role = "User" };
            var queryDto = new QueryDTO { Subject = "Subject", Message = "Message" };
            var query = new Query
            {
                CustomerId = customerId,
                Subject = "Subject",
                Message = "Message",
                Status = "Open",
                CreatedDate = DateTime.UtcNow
            };

            _mockCustomerRepository.Setup(repo => repo.GetByKey(customerId)).ReturnsAsync(customer);
            _mockQueryRepository.Setup(repo => repo.Add(It.IsAny<Query>())).ReturnsAsync(query);

            // Act
            var result = await _queryService.SubmitQueryAsync(queryDto, customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(query.Subject, result.Subject);
            Assert.AreEqual(query.Message, result.Message);
        }

        [Test]
        public void SubmitQueryAsync_CustomerNotFound_ThrowsNoSuchCustomerFoundException()
        {
            // Arrange
            var customerId = 1;
            var queryDto = new QueryDTO { Subject = "Subject", Message = "Message" };

            _mockCustomerRepository.Setup(repo => repo.GetByKey(customerId)).ReturnsAsync((Customer)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchCustomerFoundException>(() => _queryService.SubmitQueryAsync(queryDto, customerId));
        }

        [Test]
        public void SubmitQueryAsync_CustomerIsAdmin_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer { Id = customerId, Role = "Admin" };
            var queryDto = new QueryDTO { Subject = "Subject", Message = "Message" };

            _mockCustomerRepository.Setup(repo => repo.GetByKey(customerId)).ReturnsAsync(customer);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _queryService.SubmitQueryAsync(queryDto, customerId));
        }

        [Test]
        public async Task GetAllQueriesAsync_ReturnsAllQueries()
        {
            // Arrange
            var queries = new List<Query>
            {
                new Query { Id = 1, Subject = "Subject1", Message = "Message1" },
                new Query { Id = 2, Subject = "Subject2", Message = "Message2" }
            };

            _mockQueryRepository.Setup(repo => repo.Get()).ReturnsAsync(queries);

            // Act
            var result = await _queryService.GetAllQueriesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetQueryByIdAsync_ValidCustomer_ReturnsQuery()
        {
            // Arrange
            var customerId = 1;
            var queryId = 1;
            var customer = new Customer { Id = customerId, Role = "User" };
            var query = new Query { Id = queryId, CustomerId = customerId, Subject = "Subject", Message = "Message" };

            _mockCustomerRepository.Setup(repo => repo.GetByKey(customerId)).ReturnsAsync(customer);
            _mockQueryRepository.Setup(repo => repo.GetByKey(queryId)).ReturnsAsync(query);

            // Act
            var result = await _queryService.GetQueryByIdAsync(queryId, customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(queryId, result.Id);
        }

        [Test]
        public void GetQueryByIdAsync_CustomerNotFound_ThrowsNoSuchCustomerFoundException()
        {
            // Arrange
            var customerId = 1;
            var queryId = 1;

            _mockCustomerRepository.Setup(repo => repo.GetByKey(customerId)).ReturnsAsync((Customer)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchCustomerFoundException>(() => _queryService.GetQueryByIdAsync(queryId, customerId));
        }

        [Test]
        public void GetQueryByIdAsync_QueryNotFound_ThrowsNoSuchQueryFoundException()
        {
            // Arrange
            var customerId = 1;
            var queryId = 1;
            var customer = new Customer { Id = customerId, Role = "User" };

            _mockCustomerRepository.Setup(repo => repo.GetByKey(customerId)).ReturnsAsync(customer);
            _mockQueryRepository.Setup(repo => repo.GetByKey(queryId)).ReturnsAsync((Query)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchQueryFoundException>(() => _queryService.GetQueryByIdAsync(queryId, customerId));
        }

        [Test]
        public void GetQueryByIdAsync_UnauthorizedAccess_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var customerId = 1;
            var queryId = 1;
            var customer = new Customer { Id = customerId, Role = "User" };
            var query = new Query { Id = queryId, CustomerId = 2, Subject = "Subject", Message = "Message" };

            _mockCustomerRepository.Setup(repo => repo.GetByKey(customerId)).ReturnsAsync(customer);
            _mockQueryRepository.Setup(repo => repo.GetByKey(queryId)).ReturnsAsync(query);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _queryService.GetQueryByIdAsync(queryId, customerId));
        }

        [Test]
        public async Task RespondToQueryAsync_ValidQuery_RespondsSuccessfully()
        {
            // Arrange
            var queryId = 1;
            var query = new Query { Id = queryId, Subject = "Subject", Message = "Message", Status = "Open" };

            _mockQueryRepository.Setup(repo => repo.GetByKey(queryId)).ReturnsAsync(query);

            // Act
            var result = await _queryService.RespondToQueryAsync(queryId, "Response");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Query responded successfully.", result.Message);
            Assert.AreEqual("Responded", query.Status);
        }

        [Test]
        public async Task RespondToQueryAsync_QueryNotFound_ReturnsFailure()
        {
            // Arrange
            var queryId = 1;

            _mockQueryRepository.Setup(repo => repo.GetByKey(queryId)).ReturnsAsync((Query)null);

            // Act
            var result = await _queryService.RespondToQueryAsync(queryId, "Response");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Query not found.", result.Message);
        }

        [Test]
        public async Task CloseQueryAsync_ValidQuery_ClosesSuccessfully()
        {
            // Arrange
            var queryId = 1;
            var query = new Query { Id = queryId, Subject = "Subject", Message = "Message", Status = "Open" };

            _mockQueryRepository.Setup(repo => repo.GetByKey(queryId)).ReturnsAsync(query);

            // Act
            var result = await _queryService.CloseQueryAsync(queryId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Query closed successfully.", result.Message);
            Assert.AreEqual("Closed", query.Status);
        }

        [Test]
        public async Task CloseQueryAsync_QueryNotFound_ReturnsFailure()
        {
            // Arrange
            var queryId = 1;

            _mockQueryRepository.Setup(repo => repo.GetByKey(queryId)).ReturnsAsync((Query)null);

            // Act
            var result = await _queryService.CloseQueryAsync(queryId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Query not found.", result.Message);
        }
    }
}
