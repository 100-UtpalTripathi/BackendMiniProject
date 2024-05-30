using CarBookingApplication.Exceptions;
using CarBookingApplication.Exceptions.Customer;
using CarBookingApplication.Exceptions.User;
using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using CarBookingApplication.Models.DTOs.UserDTOs;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;


namespace CarBookingApplication.Services
{
    public class UserService : IUserService
    {
        #region Private Fields

        private readonly IRepository<int, User> _userRepo;
        private readonly IRepository<int, Customer> _customerRepo;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserService> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepo">The repository for user data.</param>
        /// <param name="customerRepo">The repository for customer data.</param>
        /// <param name="tokenService">The service responsible for token generation.</param>
        /// <param name="logger">The logger for logging messages.</param>
        public UserService(IRepository<int, User> userRepo, IRepository<int, Customer> customerRepo, ITokenService tokenService, ILogger<UserService> logger)
        {
            _userRepo = userRepo;
            _customerRepo = customerRepo;
            _tokenService = tokenService;
            _logger = logger;
        }

        #endregion

        #region Login
        /// <summary>
        /// Logs in a user based on the provided login credentials.
        /// </summary>
        /// <param name="loginDTO">The DTO containing user login information.</param>
        /// <returns>A DTO containing login details.</returns>

        public async Task<LoginReturnDTO> Login(UserLoginDTO loginDTO)
        {
            var userDB = await _userRepo.GetByKey(loginDTO.UserId);
            if (userDB == null)
            {
                throw new UnauthorizedUserException("Invalid username or password");
            }
            HMACSHA512 hMACSHA = new HMACSHA512(userDB.PasswordHashKey);
            var encrypterPass = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            bool isPasswordSame = ComparePassword(encrypterPass, userDB.Password);
            if (isPasswordSame)
            {
                var customer = await _customerRepo.GetByKey(loginDTO.UserId);
                if(userDB.Status =="Active")
                {
                    LoginReturnDTO loginReturnDTO = MapCustomerToLoginReturn(customer);
                    return loginReturnDTO;
                }

                throw new UserNotActiveException("Your account is not activated yet! Try Again Later.");
            }
            throw new UnauthorizedUserException("Invalid username or password");
        }

        #endregion

        #region ComparePassword


        [ExcludeFromCodeCoverage]
        public bool ComparePassword(byte[] encrypterPass, byte[] password)
        {
            for (int i = 0; i < encrypterPass.Length; i++)
            {
                if (encrypterPass[i] != password[i])
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Register
        /// <summary>
        /// Registers a new customer.
        /// </summary>
        /// <param name="customerDTO">The DTO containing customer information.</param>
        /// <returns>The registered customer.</returns>

        public async Task<Customer> Register(CustomerUserDTO customerDTO)
        {
            Customer customer = null;
            User user = null;
            try
            {
                customer = MapToCustomer(customerDTO);
                user = MapCustomerUserDTOToUser(customerDTO);
                customer = await _customerRepo.Add(customer);

                user.CustomerId = customer.Id;
                if(customer.Role != "Admin")
                {
                    user.Status = "Active";
                }
                else
                {
                    user.Status = "Disabled";
                }

                user = await _userRepo.Add(user);

                return customer;
            }
            catch (Exception) { }
            if (customer != null)
                await RevertCustomerInsert(customer);

            if (user != null && customer == null)
                await RevertUserInsert(user);

            throw new UnableToRegisterException("Not able to register at this moment!");
        }

        #endregion

        #region GetCustomerProfile


        [ExcludeFromCodeCoverage]
        private LoginReturnDTO MapCustomerToLoginReturn(Customer customer)
        {
            LoginReturnDTO returnDTO = new LoginReturnDTO();
            returnDTO.Id = customer.Id;
            returnDTO.Role = customer.Role ?? "User";
            returnDTO.Token = _tokenService.GenerateToken(customer);
            return returnDTO;
        }

        #endregion

        #region RevertUserInsert
        [ExcludeFromCodeCoverage]
        private async Task RevertUserInsert(User user)
        {
            await _userRepo.DeleteByKey(user.CustomerId);
        }

        #endregion

        #region RevertCustomerInsert

        [ExcludeFromCodeCoverage]
        private async Task RevertCustomerInsert(Customer customer)
        {

            await _customerRepo.DeleteByKey(customer.Id);
        }

        #endregion


        #region MapCustomerUserDTOToUser
        [ExcludeFromCodeCoverage]
        private User MapCustomerUserDTOToUser(CustomerUserDTO customerDTO)
        {
            User user = new User();
            user.Status = "Disabled";
            HMACSHA512 hMACSHA = new HMACSHA512();
            user.PasswordHashKey = hMACSHA.Key;
            user.Password = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(customerDTO.Password));
            return user;
        }

        #endregion

        #region UserActivation
        /// <summary>
        /// Activates or deactivates a user.
        /// </summary>
        /// <param name="userActivationDTO">The DTO containing user activation details.</param>
        /// <returns>The updated user activation status.</returns>

        //  user activation logic
        public async Task<ReturnUserActivationDTO> UserActivation(UserActivationDTO userActivationDTO)
        {
            try
            {
                var user = await _userRepo.GetByKey(userActivationDTO.UserId);
                if (user == null)
                {
                    throw new NoSuchUserFoundException("User not found.");
                }

                var customer = await _customerRepo.GetByKey(userActivationDTO.UserId);
                if (customer == null)
                {
                    throw new NoSuchCustomerFoundException("Customer not found.");
                }

                if (customer.Role != "Admin")
                {
                    throw new UnauthorizedAccessException("Only admins can activate users.");
                }

                if (userActivationDTO.IsActive)
                {
                    user.Status = "Active";

                    var updatedUser = await _userRepo.Update(user);

                    if (updatedUser == null)
                    {
                        throw new UserUpdateStatusFailedException("Failed to update user status.");
                    }

                    return new ReturnUserActivationDTO
                    {
                        UserId = user.CustomerId,
                        Status = user.Status
                    };
                }
                else
                {
                    throw new ArgumentException("User activation flag is false.");
                }
            }
            catch (NoSuchUserFoundException ex)
            {
                throw;
            }
            catch (NoSuchCustomerFoundException ex)
            {
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw new UserUpdateStatusFailedException("Unable to activate user at this moment.");
            }
        }

        #endregion


        #region MapToCustomer
        public static Customer MapToCustomer(CustomerUserDTO customerDTO)
        {
            return new Customer
            {
                Name = customerDTO.Name,
                DateOfBirth = customerDTO.DateOfBirth,
                Phone = customerDTO.Phone,
                Image = customerDTO.Image,
                Role = customerDTO.Role,
                Email = customerDTO.Email
            };
        }

        #endregion
    }
}
