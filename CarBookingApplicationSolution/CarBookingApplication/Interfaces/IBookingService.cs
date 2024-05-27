using CarBookingApplication.Models;
using CarBookingApplication.Models.DTOs.BookingDTOs;
namespace CarBookingApplication.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetAllBookingsAsync(int customerId);
        Task<Booking> GetBookingByIdAsync(int id, int customerId);
        Task<BookingResponseDTO> CancelBookingAsync(int id, int customerId);


        Task<Booking> BookCarAsync(int customerId, BookingDTO bookingRequest);


    }
}
