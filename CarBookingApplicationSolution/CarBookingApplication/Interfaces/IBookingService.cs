using CarBookingApplication.Models;
using CarBookingApplication.Models.DTOs.BookingDTOs;
namespace CarBookingApplication.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetAllBookingsAsync(int customerId);
        Task<BookingDTO> GetBookingByIdAsync(int id, int customerId);
        Task<BookingResponseDTO> CancelBookingAsync(int id);
        Task<BookingResponseDTO> HandleBookingIssueAsync(int id, string issueDetails);
    }
}
