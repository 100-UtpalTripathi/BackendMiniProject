using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using CarBookingApplication.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace CarBookingUnitTest
{
    public class TokenServiceTest
    {
        private Mock<IConfiguration> _configurationMock;

        [SetUp]
        public void Setup()
        {
            var secretKey = "This is the dummy key which has to be a bit long for the 512. which should be even more longer for the passing";

            var configurationJWTSectionMock = new Mock<IConfigurationSection>();
            configurationJWTSectionMock.Setup(x => x.Value).Returns(secretKey);

            var configurationTokenKeySectionMock = new Mock<IConfigurationSection>();
            configurationTokenKeySectionMock.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSectionMock.Object);

            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(x => x.GetSection("TokenKey")).Returns(configurationTokenKeySectionMock.Object);
        }

        [Test]
        public void CreateTokenPassTest()
        {
            // Arrange
            ITokenService service = new TokenService(_configurationMock.Object);

            // Act
            var token = service.GenerateToken(new Customer { Id = 103, Role = "Admin" });

            // Assert
            Assert.IsNotNull(token);
        }

        [Test]
        public void TokenContainsCorrectClaimsTest()
        {
            // Arrange
            ITokenService service = new TokenService(_configurationMock.Object);

            // Act
            var token = service.GenerateToken(new Customer { Id = 103, Role = "Admin" });
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Assert
            Assert.AreEqual("103", jwtToken.Claims.FirstOrDefault(c => c.Type == "eid")?.Value);
            Assert.AreEqual("Admin", jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);
        }

        [Test]
        public void TokenHasCorrectExpirationTest()
        {
            // Arrange
            ITokenService service = new TokenService(_configurationMock.Object);

            // Act
            var token = service.GenerateToken(new Customer { Id = 103, Role = "Admin" });
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Assert
            Assert.IsTrue(jwtToken.ValidTo <= DateTime.UtcNow.AddDays(2));
        }

        [Test]
        public void GenerateTokenInvalidConfigurationTest()
        {
            // Arrange
            var invalidConfigurationMock = new Mock<IConfiguration>();
            var emptyJWTSectionMock = new Mock<IConfigurationSection>();
            emptyJWTSectionMock.Setup(x => x.Value).Returns((string)null); // Simulate missing JWT value
            invalidConfigurationMock.Setup(x => x.GetSection("TokenKey:JWT")).Returns(emptyJWTSectionMock.Object);

            ITokenService service;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => service = new TokenService(invalidConfigurationMock.Object));
            Assert.AreEqual("JWT secret key is missing or empty.", ex.Message);
        }

        [Test]
        public void CreateTokenForDifferentRoleTest()
        {
            // Arrange
            ITokenService service = new TokenService(_configurationMock.Object);

            // Act
            var token = service.GenerateToken(new Customer { Id = 104, Role = "User" });
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Assert
            Assert.AreEqual("104", jwtToken.Claims.FirstOrDefault(c => c.Type == "eid")?.Value);
            Assert.AreEqual("User", jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);
        }
    }
}
