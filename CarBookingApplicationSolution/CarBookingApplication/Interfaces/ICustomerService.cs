using CarBookingApplication.Models.DTOs.BookingDTOs;
using CarBookingApplication.Models.DTOs.UserDTOs;

namespace CarBookingApplication.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerUserDTO> GetCustomerProfileAsync(int customerId);
        Task<CustomerUserDTO> UpdateCustomerProfileAsync(int customerId, CustomerUserDTO customerDTO);
        Task<IEnumerable<BookingDTO>> GetCustomerBookingsAsync(int customerId);
    }
}
