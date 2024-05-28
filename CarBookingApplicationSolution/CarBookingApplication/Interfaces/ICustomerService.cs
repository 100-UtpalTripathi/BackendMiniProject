using CarBookingApplication.Models.DTOs.BookingDTOs;
using CarBookingApplication.Models.DTOs.UserDTOs;
using CarBookingApplication.Models;

namespace CarBookingApplication.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerUserDTO> GetCustomerProfileAsync(int customerId);
        Task<CustomerUserDTO> UpdateCustomerProfileAsync(int customerId, CustomerUserDTO customerDTO);
        Task<IEnumerable<Booking>> GetCustomerBookingsAsync(int customerId);
    }
}
