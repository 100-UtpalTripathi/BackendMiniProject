using CarBookingApplication.Interfaces;
using CarBookingApplication.Models.DTOs.BookingDTOs;
using CarBookingApplication.Models;
using CarBookingApplication.Exceptions.Booking;
using System.Reflection.Metadata.Ecma335;
using CarBookingApplication.Exceptions.Car;
using CarBookingApplication.Exceptions.Customer;

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


        public async Task<Booking> GetBookingByIdAsync(int bookingId, int customerId)
        {
            try
            {
                var customer = await _customerRepository.GetByKey(customerId);

                var storedBooking = await _bookingRepository.GetByKey(bookingId);

                if (customer.Role != "Admin" && storedBooking.CustomerId != customerId)
                {
                    throw new UnauthorizedAccessException("You are not authorized to view this booking.");
                }

                return storedBooking;
            }
            catch (UnauthorizedAccessException)
            { throw; }
        }

        public async Task<BookingResponseDTO> CancelBookingAsync(int bookingId, int customerId)
        {
            try
            {
                // Get the booking by its ID
                var booking = await _bookingRepository.GetByKey(bookingId);

                // Check if the booking exists
                if (booking == null)
                {
                    return new BookingResponseDTO
                    {
                        Message = $"Booking with ID {bookingId} not found.",
                        Success = false
                    };
                }

                // Check if the customer ID matches the booking's customer ID
                if (booking.CustomerId != customerId)
                {
                    // If not, return an unauthorized access response
                    return new BookingResponseDTO
                    {
                        Message = "You are not authorized to cancel this booking.",
                        Success = false
                    };
                }

                if (booking.Status == "Cancelled")
                {
                    return new BookingResponseDTO
                    {
                        Message = "Booking has already been cancelled.",
                        Success = false
                    };
                }

                if (booking.StartDate < DateTime.Now)
                {
                    return new BookingResponseDTO
                    {
                        Message = "Booking has already started. Cannot cancel.",
                        Success = false
                    };
                }

                // Calculate the time difference between the booking start date and the current date
                var timeDifference = booking.StartDate.Subtract(DateTime.Now);

                // Check if the cancellation falls within the 48-hour window
                if (timeDifference.TotalHours >= 48)
                {
                    // No cancellation fee is applied
                    booking.Status = "Cancelled";
                    await _bookingRepository.Update(booking);

                    return new BookingResponseDTO
                    {
                        Message = "Booking cancelled successfully.",
                        Success = true
                    };
                }
                else
                {

                    decimal cancellationFee = CalculateCancellationFee(timeDifference);

                    // Update the booking with the cancellation fee and status
                    booking.Status = "Cancelled";
                    booking.FinalAmount -= cancellationFee; // Deduct the cancellation fee from the final amount
                    await _bookingRepository.Update(booking);

                    return new BookingResponseDTO
                    {
                        Message = "Booking cancelled successfully. Cancellation fee applied.",
                        Success = true
                    };
                }
            }
            catch (Exception ex)
            {
                // Log and return a generic error response
                return new BookingResponseDTO
                {
                    Message = "An unexpected error occurred while cancelling the booking.",
                    Success = false
                };
            }
        }

        public async Task<Booking> BookCarAsync(int customerId, BookingDTO bookingRequest)
        {
            if (bookingRequest.BookingDate < DateTime.Now)
            {
                throw new InvalidBookingDate("Booking date cannot be in the past.");
            }
            var car = await _carRepository.GetByKey(bookingRequest.CarId);
            if (car == null)
            {
                throw new NoSuchCarFoundException("Car not found!.");
            }

            if (car.Status != "Available")
            {
                throw new CarNotAvailableForBookingException("Car is not available for booking.");
            }

            var customer = await _customerRepository.GetByKey(customerId);
            if (customer == null)
            {
                throw new NoSuchCustomerFoundException("Customer not found!");
            }

            var totalAmount = CalculateTotalAmount(car, bookingRequest.StartDate, bookingRequest.EndDate);
            var discountAmount = CalculateDiscountAmount(customer, bookingRequest.StartDate, totalAmount);
            var finalAmount = totalAmount - discountAmount;

            var booking = new Booking
            {
                CarId = bookingRequest.CarId,
                CustomerId = customerId,
                BookingDate = bookingRequest.BookingDate,
                StartDate = bookingRequest.StartDate,
                EndDate = bookingRequest.EndDate,
                TotalAmount = totalAmount,
                DiscountAmount = discountAmount,
                FinalAmount = finalAmount,
                Status = "Confirmed"
            };

            booking = await _bookingRepository.Add(booking);

            car.Status = "Booked";   // marking Car as Booked, and updating info
            await _carRepository.Update(car);

            return booking;
        }

        private decimal CalculateTotalAmount(Car car, DateTime startDate, DateTime endDate)
        {
            decimal dailyRate = 1000; // Example rate
            int totalDays = (endDate - startDate).Days;
            return dailyRate * totalDays;
        }

        private decimal CalculateDiscountAmount(Customer customer, DateTime startDate, decimal totalAmount)
        {
            decimal discountAmount = 0;

            // Applying seasonal discount
            if (startDate.Month == 12)
            {
                discountAmount += totalAmount * 0.10m; // 10% off in December, m is used to represent decimal value decimal
            }

            // Applying loyalty discount based on more than 5 bookings in same year
            var customerBookings = customer.Bookings.Where(b => b.BookingDate.Year == startDate.Year).Count();
            if (customerBookings > 5)
            {
                discountAmount += totalAmount * 0.05m; // 5% off for more than 5 bookings
            }

            return discountAmount;
        }

        private decimal CalculateCancellationFee(TimeSpan timeDifference)
        {
            // Calculate the remaining hours until the booking start date
            double remainingHours = 48 - timeDifference.TotalHours;

            // If the remaining hours are less than or equal to zero, no cancellation fee is applied
            if (remainingHours <= 0)
            {
                return 0;
            }
            else
            {
                decimal cancellationFee = (decimal)remainingHours * 40; // Assuming $40 per hour
                return cancellationFee;
            }
        }
    }
}
