using CarBookingApplication.Contexts;
using CarBookingApplication.Exceptions.Customer;
using CarBookingApplication.Models;
using CarBookingApplication.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace CarBookingUnitTest
{
    public class CustomerRepositoryTest
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
                context.Customers.AddRange(
                    new Customer { Id = 1, Name = "Somu", DateOfBirth = new DateTime(1990, 1, 1), Phone = "1234567890", Email = "somu@example.com", Role = "User" },
                    new Customer { Id = 2, Name = "Aruna", DateOfBirth = new DateTime(1985, 5, 5), Phone = "9876543210", Email = "aruna@example.com", Role = "Admin" }
                );
                context.SaveChanges();
            }
        }

        [Test]
        public async Task AddCustomerAsync_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CustomerRepository(context);
                var customer = new Customer { Id = 3, Name = "Antony", DateOfBirth = new DateTime(1980, 10, 10), Phone = "5555555555", Email = "antony@example.com", Role = "User" };

                // Act
                var addedCustomer = await repository.Add(customer);

                // Assert
                Assert.IsNotNull(addedCustomer);
                Assert.AreEqual(customer.Id, addedCustomer.Id);
            }
        }

        [Test]
        public async Task DeleteCustomerByKey_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CustomerRepository(context);

                // Add a customer to the database
                var customer = new Customer { Id = 4, Name = "Amrita", DateOfBirth = DateTime.Parse("1990-01-01"), Phone = "1234567890", Email = "amrita@example.com", Role = "User" };
                await context.Customers.AddAsync(customer);
                await context.SaveChangesAsync();

                // Act
                var deletedCustomer = await repository.DeleteByKey(4);

                // Assert
                Assert.IsNotNull(deletedCustomer);
                Assert.AreEqual(customer.Id, deletedCustomer.Id);
            }
        }

        [Test]
        public async Task DeleteCustomerByKey_CustomerNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CustomerRepository(context);

                // Act & Assert
                var ex = Assert.ThrowsAsync<NoSuchCustomerFoundException>(() => repository.DeleteByKey(4));
                Assert.AreEqual("No Customer found with given ID!", ex.Message);
            }
        }

        [Test]
        public async Task GetCustomerByKey_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CustomerRepository(context);

                // Act
                var customer = await repository.GetByKey(1);

                // Assert
                Assert.IsNotNull(customer);
                Assert.AreEqual(1, customer.Id);
            }
        }

        [Test]
        public async Task GetCustomerByKey_CustomerNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CustomerRepository(context);

                // Act & Assert
                var customer = await repository.GetByKey(100); // Assuming ID 100 does not exist
                Assert.IsNull(customer);
            }
        }


        [Test]
        public async Task GetCustomers_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CustomerRepository(context);

                // Act
                var customers = await repository.Get();

                // Assert
                Assert.IsNotNull(customers);
                Assert.AreEqual(2, customers.Count()); // Assuming 2 customers were seeded in the database
            }
        }

        [Test]
        public async Task GetCustomers_Empty()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CarBookingContext>()
                .UseInMemoryDatabase(databaseName: "empty_database")
                .Options;

            using (var context = new CarBookingContext(options))
            {
                var repository = new CustomerRepository(context);

                // Act
                var customers = await repository.Get();

                // Assert
                Assert.IsNotNull(customers);
                Assert.IsEmpty(customers);
            }
        }

        [Test]
        public async Task UpdateCustomer_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CustomerRepository(context);
                var customer = new Customer { Id = 1, Name = "Albatross", DateOfBirth = new DateTime(1990, 1, 1), Phone = "1234567890", Email = "albatross@example.com", Role = "User" };

                // Act
                var updatedCustomer = await repository.Update(customer);

                // Assert
                Assert.IsNotNull(updatedCustomer);
                Assert.AreEqual(customer.Id, updatedCustomer.Id);
                Assert.AreEqual(customer.Name, updatedCustomer.Name);
            }
        }

        [Test]
        public async Task UpdateCustomer_CustomerNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new CustomerRepository(context);
                var invalidCustomer = new Customer { Id = 100, Name = "Invalid", DateOfBirth = DateTime.Now, Phone = "1234567890", Email = "invalid@example.com", Role = "User" };

                // Act & Assert
                var exception = Assert.ThrowsAsync<NoSuchCustomerFoundException>(() => repository.Update(invalidCustomer));
                Assert.AreEqual("No Customer found with given ID!", exception.Message);
            }
        }


    }
}
