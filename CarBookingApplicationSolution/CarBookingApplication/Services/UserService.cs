using CarBookingApplication.Exceptions;
using CarBookingApplication.Exceptions.Customer;
using CarBookingApplication.Exceptions.User;
using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using CarBookingApplication.Models.DTOs.UserDTOs;
using System.Security.Cryptography;
using System.Text;


namespace CarBookingApplication.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<int, User> _userRepo;
        private readonly IRepository<int, Customer> _customerRepo;
        private readonly ITokenService _tokenService;

        public UserService(IRepository<int, User> userRepo, IRepository<int, Customer> customerRepo, ITokenService tokenService)
        {
            _userRepo = userRepo;
            _customerRepo = customerRepo;
            _tokenService = tokenService;
        }
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

        private LoginReturnDTO MapCustomerToLoginReturn(Customer customer)
        {
            LoginReturnDTO returnDTO = new LoginReturnDTO();
            returnDTO.CustomerID = customer.Id;
            returnDTO.Role = customer.Role ?? "User";
            returnDTO.Token = _tokenService.GenerateToken(customer);
            return returnDTO;
        }

        private async Task RevertUserInsert(User user)
        {
            await _userRepo.DeleteByKey(user.CustomerId);
        }

        private async Task RevertCustomerInsert(Customer customer)
        {

            await _customerRepo.DeleteByKey(customer.Id);
        }

        private User MapCustomerUserDTOToUser(CustomerUserDTO customerDTO)
        {
            User user = new User();
            user.Status = "Disabled";
            HMACSHA512 hMACSHA = new HMACSHA512();
            user.PasswordHashKey = hMACSHA.Key;
            user.Password = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(customerDTO.Password));
            return user;
        }




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
                // Log generic exception
                throw new UserUpdateStatusFailedException("Unable to activate user at this moment.");
            }
        }


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
    }
}
