using CarBookingApplication.Contexts;
using CarBookingApplication.Exceptions.User;
using CarBookingApplication.Models;
using CarBookingApplication.Repositories;
using Microsoft.EntityFrameworkCore;


namespace CarBookingUnitTest
{
    public class UserRepositoryTest
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
                context.Users.AddRange(
                    new User { CustomerId = 1, Password = new byte[] { 0x01, 0x02, 0x03 }, PasswordHashKey = new byte[] { 0x04, 0x05, 0x06 }, Status = "Enabled" },
                    new User { CustomerId = 2, Password = new byte[] { 0x07, 0x08, 0x09 }, PasswordHashKey = new byte[] { 0x0A, 0x0B, 0x0C }, Status = "Enabled" }
                );
                context.SaveChanges();
            }
        }

        [Test]
        public async Task AddUserAsync_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new UserRepository(context);
                var user = new User { CustomerId = 3, Password = new byte[] { 0x0D, 0x0E, 0x0F }, PasswordHashKey = new byte[] { 0x10, 0x11, 0x12 }, Status = "Enabled" };

                // Act
                var addedUser = await repository.Add(user);

                // Assert
                Assert.IsNotNull(addedUser);
                Assert.AreEqual(user.CustomerId, addedUser.CustomerId);
            }
        }

        [Test]
        public async Task DeleteUserByKey_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new UserRepository(context);

                // Add a user to the database
                var user = new User { CustomerId = 4, Password = new byte[] { 0x13, 0x14, 0x15 }, PasswordHashKey = new byte[] { 0x16, 0x17, 0x18 }, Status = "Enabled" };
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                // Act
                var deletedUser = await repository.DeleteByKey(4);

                // Assert
                Assert.IsNotNull(deletedUser);
                Assert.AreEqual(user.CustomerId, deletedUser.CustomerId);
            }
        }

        [Test]
        public async Task DeleteUserByKey_UserNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new UserRepository(context);

                // Act & Assert
                var ex = Assert.ThrowsAsync<NoSuchUserFoundException>(() => repository.DeleteByKey(4));
                Assert.AreEqual("No user found with the given ID", ex.Message);
            }
        }


        [Test]
        public async Task GetUserByKey_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new UserRepository(context);

                // Act
                var user = await repository.GetByKey(1);

                // Assert
                Assert.IsNotNull(user);
                Assert.AreEqual(1, user.CustomerId);
            }
        }

        [Test]
        public async Task GetUserByKey_UserNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new UserRepository(context);

                // Act & Assert
                var user = await repository.GetByKey(100); // Assuming ID 100 does not exist
                Assert.IsNull(user);
            }
        }

        [Test]
        public async Task GetAllUsers_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new UserRepository(context);

                // Act
                var users = await repository.Get();

                // Assert
                Assert.IsNotNull(users);
                Assert.AreEqual(2, users.Count());
            }
        }

        [Test]
        public async Task UpdateUser_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new UserRepository(context);

                // Get an existing user
                var user = await repository.GetByKey(1);
                user.Status = "Disabled"; // Change the status

                // Act
                var updatedUser = await repository.Update(user);

                // Assert
                Assert.IsNotNull(updatedUser);
                Assert.AreEqual("Disabled", updatedUser.Status);
            }
        }

        [Test]
        public async Task UpdateUser_UserNotFound()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new UserRepository(context);

                // Create a user with a non-existing ID
                var user = new User { CustomerId = 100, Password = new byte[] { 0x19, 0x1A, 0x1B }, PasswordHashKey = new byte[] { 0x1C, 0x1D, 0x1E }, Status = "Enabled" };

                // Act & Assert
                var ex = Assert.ThrowsAsync<NoSuchUserFoundException>(() => repository.Update(user));
                Assert.AreEqual("No User found with given ID!", ex.Message);
            }
        }
    }
}
