using CarBookingApplication.Models.DTOs.BookingDTOs;
using CarBookingApplication.Models.DTOs.UserDTOs;
using CarBookingApplication.Models;
using CarBookingApplication.Models.DTOs.CarRatingDTOs;

namespace CarBookingApplication.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerUserDTO> GetCustomerProfileAsync(int customerId);
        Task<CustomerUserDTO> UpdateCustomerProfileAsync(int customerId, CustomerUserDTO customerDTO);
        Task<IEnumerable<Booking>> GetCustomerBookingsAsync(int customerId);



        Task<CarRating> AddRatingAsync(int customerId, CarRatingDTO carRatingDTO);

    }
}
