using CarBookingApplication.Contexts;
using CarBookingApplication.Exceptions.User;
using CarBookingApplication.Models;
using CarBookingApplication.Repositories;
using Microsoft.EntityFrameworkCore;


namespace CarBookingUnitTest.RepositoryTest
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
                var user = new User { CustomerId = 5,Password = new byte[] { 0x07, 0x08, 0x09 }, PasswordHashKey = new byte[] { 0x0A, 0x0B, 0x0C }, Status = "Enabled" };

                // Add the user and await the result
                var addedUser = await repository.Add(user);

                // Act
                var getUser = await repository.GetByKey(addedUser.CustomerId); // Use the Id property to retrieve the user

                // Assert
                Assert.IsNotNull(getUser);
                Assert.AreEqual(addedUser.CustomerId, getUser.CustomerId); // Compare Id instead of CustomerId
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
                Assert.AreEqual(1, users.Count());
            }
        }

        [Test]
        public async Task UpdateUser_Success()
        {
            // Arrange
            using (var context = new CarBookingContext(_options))
            {
                var repository = new UserRepository(context);

                var user = new User { CustomerId = 1, Password = new byte[] { 0x07, 0x08, 0x09 }, PasswordHashKey = new byte[] { 0x0A, 0x0B, 0x0C }, Status = "Enabled" };
                var addedUser = await repository.Add(user);
                // Get an existing user
                var newUser = await repository.GetByKey(addedUser.CustomerId);
                newUser.Status = "Disabled"; // Change the status

                // Act
                var updatedUser = await repository.Update(newUser);

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
