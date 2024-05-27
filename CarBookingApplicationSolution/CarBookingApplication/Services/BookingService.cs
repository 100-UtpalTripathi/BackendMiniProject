using CarBookingApplication.Interfaces;
using CarBookingApplication.Models.DTOs.BookingDTOs;
using CarBookingApplication.Models;
using CarBookingApplication.Exceptions.Booking;

namespace CarBookingApplication.Services
{
    public class BookingService : IBookingService
    {
        private readonly IRepository<int, Booking> _bookingRepository;
        private readonly IRepository<int, Customer> _customerRepository;
        private readonly IRepository<int, Car> _carRepository;

        public BookingService(IRepository<int, Booking> bookingRepository, IRepository<int, Customer> customerRepository, IRepository<int, Car> CarRepository)
        {
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _carRepository = CarRepository;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync(int customerId)
        {
            try
            {
                var customer = await _customerRepository.GetByKey(customerId);

                if (customer.Role == "Admin")
                {
                    // Retrieving all bookings if the customer is an admin
                    var allBookings = await _bookingRepository.Get();
                    return allBookings;
                }
                else
                {
                    // Retrieve bookings specific to the customer
                    var bookings = await _bookingRepository.Get();
                    var customerBookings = bookings.Where(b => b.CustomerId == customerId);
                    return customerBookings;
                }
            }
            catch (Exception)
            {
                throw new NoBookingsFoundException("Error occurred while retrieving bookings.");
            }
            
        }


        public async Task<Booking> GetBookingByIdAsync(int id, int customerId)
        {
            try
            {
                var customer = await _customerRepository.GetByKey(customerId);

                
            }
            catch (Exception)
            {
                throw new NoBookingsFoundException("Error occurred while retrieving bookings.");
            }


            var booking = await _bookingRepository.GetByKey(id);
            if (booking == null)
            {
                return null;
            }

            return new BookingDTO
            {
                Id = booking.Id,
                CarId = booking.CarId,
                CustomerId = booking.CustomerId,
                BookingDate = booking.BookingDate,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                TotalAmount = booking.TotalAmount,
                DiscountAmount = booking.DiscountAmount,
                FinalAmount = booking.FinalAmount,
                Status = booking.Status
            };
        }

        Task<BookingResponseDTO> IBookingService.CancelBookingAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<BookingResponseDTO> IBookingService.HandleBookingIssueAsync(int id, string issueDetails)
        {
            throw new NotImplementedException();
        }
    }

}
