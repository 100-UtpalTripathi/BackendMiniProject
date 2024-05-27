using CarBookingApplication.Models;
using CarBookingApplication.Models.DTOs.UserDTOs;

namespace CarBookingApplication.Interfaces
{
    public interface IUserService
    {
        public Task<LoginReturnDTO> Login(UserLoginDTO loginDTO);
        public Task<Customer> Register(CustomerUserDTO employeeDTO);

        public Task<ReturnUserActivationDTO> UserActivation(UserActivationDTO userActivationDTO);
    }
}
