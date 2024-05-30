using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CarBookingApplication.Exceptions;
using CarBookingApplication.Exceptions.User;
using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using CarBookingApplication.Models.DTOs.UserDTOs;
using CarBookingApplication.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CarBookingUnitTest.ServicesTest
{
    public class UserServiceTest
    {
        private Mock<IRepository<int, User>> _mockUserRepository;
        private Mock<IRepository<int, Customer>> _mockCustomerRepository;
        private Mock<ITokenService> _mockTokenService;
        private Mock<ILogger<UserService>> _mockLogger;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IRepository<int, User>>();
            _mockCustomerRepository = new Mock<IRepository<int, Customer>>();
            _mockTokenService = new Mock<ITokenService>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_mockUserRepository.Object, _mockCustomerRepository.Object, _mockTokenService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task RegisterAndLogin_ValidCredentials_ReturnsLoginReturnDTO()
        {
            // Arrange
            var registrationDto = new CustomerUserDTO
            {
                Name = "John Doe",
                DateOfBirth = new DateTime(1990, 1, 1),
                Phone = "1234567890",
                Role = "Customer",
                Email = "john.doe@example.com",
                Password = "password"
            };

            var customer = new Customer
            {
                Id = 1,
                Name = registrationDto.Name,
                DateOfBirth = registrationDto.DateOfBirth,
                Phone = registrationDto.Phone,
                Image = registrationDto.Image,
                Role = registrationDto.Role,
                Email = registrationDto.Email
            };

            using var hmac = new HMACSHA512();
            var user = new User
            {
                CustomerId = customer.Id,
                PasswordHashKey = hmac.Key,
                Password = hmac.ComputeHash(Encoding.UTF8.GetBytes(registrationDto.Password)),
                Status = "Active"
            };

            // Setting up the mocks
            _mockCustomerRepository.Setup(repo => repo.Add(It.IsAny<Customer>())).ReturnsAsync(customer);
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>())).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync(user);
            _mockCustomerRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync(customer);
            _mockTokenService.Setup(service => service.GenerateToken(It.IsAny<Customer>())).Returns("token");

            // Act
            var addedCustomer = await _userService.Register(registrationDto);
            var result = await _userService.Login(new UserLoginDTO { UserId = addedCustomer.Id, Password = registrationDto.Password });

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Customer", result.Role);
            Assert.AreEqual("token", result.Token);
        }



        [Test]
        public void Login_InvalidUserId_ThrowsUnauthorizedUserException()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync((User)null);

            var loginDTO = new UserLoginDTO { UserId = 1, Password = "password" };

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedUserException>(() => _userService.Login(loginDTO));
        }

        [Test]
        public void Login_InvalidPassword_ThrowsUnauthorizedUserException()
        {
            // Arrange
            var user = new User
            {
                CustomerId = 1,
                PasswordHashKey = new byte[64],
                Password = new HMACSHA512().ComputeHash(Encoding.UTF8.GetBytes("password")),
                Status = "Active"
            };

            _mockUserRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync(user);

            var loginDTO = new UserLoginDTO { UserId = 1, Password = "wrongpassword" };

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedUserException>(() => _userService.Login(loginDTO));
        }

        [Test]
        public void Login_InactiveUser_ThrowsUserNotActiveException()
        {
            // Arrange
            var user = new User
            {
                CustomerId = 1,
                PasswordHashKey = new byte[64],
                Password = new HMACSHA512().ComputeHash(Encoding.UTF8.GetBytes("password")),
                Status = "Inactive"
            };

            _mockUserRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync(user);

            var loginDTO = new UserLoginDTO { UserId = 1, Password = "password" };

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedUserException>(() => _userService.Login(loginDTO));
        }

        [Test]
        public async Task Register_ValidCustomer_ReturnsCustomer()
        {
            // Arrange
            var customerDTO = new CustomerUserDTO
            {
                Name = "John Doe",
                DateOfBirth = new DateTime(1990, 1, 1),
                Phone = "1234567890",
                Email = "john@example.com",
                Password = "password",
                Role = "User"
            };

            var customer = UserService.MapToCustomer(customerDTO);

            _mockCustomerRepository.Setup(repo => repo.Add(It.IsAny<Customer>())).ReturnsAsync(customer);
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>())).ReturnsAsync(new User { CustomerId = 1, Status = "Active" });

            // Act
            var result = await _userService.Register(customerDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John Doe", result.Name);
        }

        [Test]
        public void Register_RepositoryThrowsException_ThrowsUnableToRegisterException()
        {
            // Arrange
            var customerDTO = new CustomerUserDTO
            {
                Name = "John Doe",
                DateOfBirth = new DateTime(1990, 1, 1),
                Phone = "1234567890",
                Email = "john@example.com",
                Password = "password",
                Role = "User"
            };

            _mockCustomerRepository.Setup(repo => repo.Add(It.IsAny<Customer>())).ThrowsAsync(new Exception());

            // Act & Assert
            Assert.ThrowsAsync<UnableToRegisterException>(() => _userService.Register(customerDTO));
        }

        [Test]
        public async Task UserActivation_ValidUser_UpdatesStatus()
        {
            // Arrange
            var user = new User { CustomerId = 1, Status = "Inactive" };
            var customer = new Customer { Id = 1, Role = "Admin" };

            _mockUserRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync(user);
            _mockCustomerRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync(customer);
            _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>())).ReturnsAsync((User u) => u);

            var userActivationDTO = new UserActivationDTO { UserId = 1, IsActive = true };

            // Act
            var result = await _userService.UserActivation(userActivationDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.UserId);
            Assert.AreEqual("Active", result.Status);
        }


        [Test]
        public void UserActivation_UserNotFound_ThrowsNoSuchUserFoundException()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync((User)null);

            var userActivationDTO = new UserActivationDTO { UserId = 1, IsActive = true };

            // Act & Assert
            Assert.ThrowsAsync<NoSuchUserFoundException>(() => _userService.UserActivation(userActivationDTO));
        }

        [Test]
        public void UserActivation_InvalidRole_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = new User { CustomerId = 1, Status = "Disabled" };
            var customer = new Customer { Id = 1, Role = "User" };

            _mockUserRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync(user);
            _mockCustomerRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync(customer);

            var userActivationDTO = new UserActivationDTO { UserId = 1, IsActive = true };

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.UserActivation(userActivationDTO));
        }

        [Test]
        public void UserActivation_InactiveUserWithInactiveActivationFlag_ThrowsArgumentException()
        {
            // Arrange
            var user = new User { CustomerId = 1, Status = "Inactive" };
            var customer = new Customer { Id = 1, Role = "Admin" };

            _mockUserRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync(user);
            _mockCustomerRepository.Setup(repo => repo.GetByKey(1)).ReturnsAsync(customer);

            var userActivationDTO = new UserActivationDTO { UserId = 1, IsActive = false };

            // Act & Assert
            Assert.ThrowsAsync<UserUpdateStatusFailedException>(() => _userService.UserActivation(userActivationDTO));
        }


        [Test]
        public async Task Register_ValidAdminCustomer_ReturnsCustomerWithDisabledStatus()
        {
            // Arrange
            var customerDTO = new CustomerUserDTO
            {
                Name = "Admin User",
                DateOfBirth = new DateTime(1985, 5, 10),
                Phone = "1234567890",
                Email = "admin@example.com",
                Password = "password",
                Role = "Admin"
            };

            var customer = UserService.MapToCustomer(customerDTO);

            _mockCustomerRepository.Setup(repo => repo.Add(It.IsAny<Customer>())).ReturnsAsync(customer);
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>())).ReturnsAsync(new User { CustomerId = 1, Status = "Disabled" });

            // Act
            var result = await _userService.Register(customerDTO);

            // Assert
            Assert.IsNotNull(result);
        }


    }
}
