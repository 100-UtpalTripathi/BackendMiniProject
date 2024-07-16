using CarBookingApplication.Interfaces;
using CarBookingApplication.Models.DTOs.BookingDTOs;
using CarBookingApplication.Models;
using CarBookingApplication.Exceptions.Booking;
using System.Reflection.Metadata.Ecma335;
using CarBookingApplication.Exceptions.Car;
using CarBookingApplication.Exceptions.Customer;
using System.Diagnostics.CodeAnalysis;

namespace CarBookingApplication.Services
{
    public class BookingService : IBookingService
    {
        #region Private Members

        private readonly IRepository<int, Booking> _bookingRepository;
        private readonly IRepository<int, Customer> _customerRepository;
        private readonly IRepository<int, Car> _carRepository;
        private readonly ILogger<BookingService> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingService"/> class.
        /// </summary>
        /// <param name="bookingRepository">The booking repository.</param>
        /// <param name="customerRepository">The customer repository.</param>
        /// <param name="CarRepository">The car repository.</param>
        /// <param name="logger">The logger.</param>
        public BookingService(IRepository<int, Booking> bookingRepository, IRepository<int, Customer> customerRepository, IRepository<int, Car> CarRepository, ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _carRepository = CarRepository;
            _logger = logger;
        }

        #endregion


        #region Get-All-Bookings
        /// <summary>
        /// Retrieves all bookings asynchronously based on the customer ID.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>A collection of bookings.</returns>
        public async Task<IEnumerable<Booking>> GetAllBookingsAsync(int customerId)
        {
            try
            {
                var customer = await _customerRepository.GetByKey(customerId);
                if(customer == null)
                {
                    throw new NoSuchCustomerFoundException("Customer not found.");
                }
                else if(customer.Role == "Admin")
                {
                    // Retrieve all bookings for the admin
                    return await _bookingRepository.Get();
                }
                else
                {
                    // Retrieve bookings specific to the customer
                    var bookings = await _bookingRepository.Get();
                    var customerBookings = bookings.Where(b => b.CustomerId == customerId);
                    return customerBookings;
                }
            }
            catch (NoSuchCustomerFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new NoBookingsFoundException("Error occurred while retrieving bookings.");
            }

        }

        #endregion

        #region Get-Booking-By-ID
        /// <summary>
        /// Retrieves a booking by its ID asynchronously.
        /// </summary>
        /// <param name="bookingId">The booking identifier.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>The booking details.</returns>

        public async Task<Booking> GetBookingByIdAsync(int bookingId, int customerId)
        {
            try
            {
                var customer = await _customerRepository.GetByKey(customerId);

                var storedBooking = await _bookingRepository.GetByKey(bookingId);
               
                if(storedBooking == null)
                {
                    throw new NoSuchBookingFoundException();
                }

                if (customer.Role != "Admin" && storedBooking.CustomerId != customerId)
                {
                    throw new UnauthorizedAccessException("You are not authorized to view this booking.");
                }

                return storedBooking;
            }
            catch(NoSuchBookingFoundException)
            { throw; }
            catch (UnauthorizedAccessException)
            { throw; }
        }

        #endregion


        #region Cancel-Booking
        /// <summary>
        /// Cancels a booking asynchronously.
        /// </summary>
        /// <param name="bookingId">The booking identifier.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>The cancellation result.</returns>

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

                var customer = await _customerRepository.GetByKey(customerId);

                // Check if it is not admin and customer ID matches the booking's customer ID
                if (customer.Role == "Customer" && booking.CustomerId != customerId)
                {
                   
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

                // Check if the cancellation falls within the 48-hour window or the customer is an admin
                if (timeDifference.TotalHours >= 48 || customer.Role == "Admin")
                {
                    // No cancellation fee is applied
                    booking.Status = "Cancelled";
                    await _bookingRepository.Update(booking);

                    // Get the corresponding car and update its status to "Available"
                    var car = await _carRepository.GetByKey(booking.CarId);
                    if (car != null)
                    {
                        car.Status = "Available";
                        await _carRepository.Update(car);
                    }

                    return new BookingResponseDTO
                    {
                        Message = "Booking cancelled successfully.",
                        Success = true
                    };
                }
                else
                {
                    var car = await _carRepository.GetByKey(booking.CarId);
                    if (car != null)
                    {
                        car.Status = "Available";
                        await _carRepository.Update(car);
                    }

                    decimal cancellationFee = CalculateCancellationFee(timeDifference, car);

                    // Update the booking with the cancellation fee and status
                    booking.Status = "Cancelled";
                    booking.FinalAmount -= cancellationFee; // Deduct the cancellation fee from the final amount

                    booking.FinalAmount = booking.FinalAmount < 0 ? 0 : booking.FinalAmount; // Final amount cannot be negative

                    await _bookingRepository.Update(booking);

                    // Get the corresponding car and update its status to "Available"
                    

                    return new BookingResponseDTO
                    {
                        Message = "Booking cancelled successfully. Cancellation fee applied.",
                        Success = true
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return new BookingResponseDTO
                {
                    Message = "An unexpected error occurred while cancelling the booking.",
                    Success = false
                };
            }
        }

        #endregion


        #region Book-Car
        /// <summary>
        /// Books a car asynchronously.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="bookingRequest">The booking request details.</param>
        /// <returns>The booking details.</returns>
        public async Task<Booking> BookCarAsync(int customerId, BookingDTO bookingRequest)
        {
            
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

        #endregion

        #region Calculate-Total-Amount
        /// <summary>
        /// Calculates the total amount for the booking.
        /// </summary>
        /// <param name="car">The car being booked.</param>
        /// <param name="startDate">The start date of the booking.</param>
        /// <param name="endDate">The end date of the booking.</param>
        /// <returns>The total amount.</returns>

        [ExcludeFromCodeCoverage]
        private decimal CalculateTotalAmount(Car car, DateTime startDate, DateTime endDate)
        {
            decimal dailyRate = (decimal)car.PricePerDay;
            int totalDays = (endDate - startDate).Days;
            return dailyRate * totalDays;
        }

        #endregion

        #region Calculate-Discount-Amount
        /// <summary>
        /// Calculates the discount amount for the booking.
        /// </summary>
        /// <param name="customer">The customer making the booking.</param>
        /// <param name="startDate">The start date of the booking.</param>
        /// <param name="totalAmount">The total amount for the booking.</param>
        /// <returns>The discount amount.</returns>

        [ExcludeFromCodeCoverage]
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

        #endregion


        #region Calculate-Cancellation-Fee
        /// <summary>
        /// Calculates the cancellation fee for the booking.
        /// </summary>
        /// <param name="timeDifference">The time difference between the booking start date and current date.</param>
        /// <returns>The cancellation fee.</returns>

        [ExcludeFromCodeCoverage]
        private decimal CalculateCancellationFee(TimeSpan timeDifference, Car car)
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
                decimal rate  = ((decimal)car.PricePerDay / 24) * 1.2m; // 1.2 times the hourly rate
                decimal cancellationFee = (decimal)remainingHours * rate; // Assuming $40 per hour
                return cancellationFee;
            }
        }

        #endregion

        #region Extend-Booking
        /// <summary>
        /// Extends the booking timings asynchronously.
        /// </summary>
        /// <param name="bookingId">The booking identifier.</param>
        /// <param name="newEndDate">The new end date.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>The booking extension result.</returns>

        public async Task<BookingResponseDTO> ExtendBookingAsync(int bookingId, DateTime newEndDate, int customerId)
        {
            try
            {
                var booking = await _bookingRepository.GetByKey(bookingId);

                if (booking == null)
                {
                    throw new NoSuchBookingFoundException($"Booking with ID {bookingId} not found.");
                }

                if(booking.Status == "Cancelled")
                {
                    throw new NoSuchBookingFoundException("Cannot extend a cancelled booking.");
                }

                if (booking.CustomerId != customerId)
                {
                    throw new UnauthorizedAccessException("You are not authorized to extend this booking.");
                }

                if (newEndDate <= booking.EndDate)
                {
                    throw new InvalidExtensionDateException("New end date must be after the current end date.");
                }

                // Calculate the additional amount for the extended period
                decimal additionalAmount = await CalculateAdditionalAmount(booking, newEndDate);

                // Update booking details
                booking.EndDate = newEndDate;
                booking.TotalAmount += additionalAmount;
                booking.FinalAmount += additionalAmount;

                await _bookingRepository.Update(booking);

                return new BookingResponseDTO
                {
                    Success = true,
                    Message = "Booking timings extended successfully."
                };
            }
            catch(NoSuchBookingFoundException ex)
            {
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                throw;
            }
            catch (InvalidExtensionDateException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return new BookingResponseDTO
                {
                    Success = false,
                    Message = "An error occurred while extending booking timings."
                };
            }
        }

        #endregion

        #region Calculate-Additional-Amount
        /// <summary>
        /// Calculates the additional amount for extending the booking.
        /// </summary>
        /// <param name="booking">The booking to be extended.</param>
        /// <param name="newEndDate">The new end date for the extended booking.</param>
        /// <returns>The additional amount.</returns>

        [ExcludeFromCodeCoverage]
        private async Task<decimal> CalculateAdditionalAmount(Booking booking, DateTime newEndDate)
        {
            // Calculate the difference in days between the current end date and the new end date
            int additionalDays = (int)(newEndDate - booking.EndDate).TotalDays;

            var car = await _carRepository.GetByKey(booking.CarId);


            decimal dailyRate = (decimal)car.PricePerDay;
            decimal additionalAmount = dailyRate * additionalDays;

            return additionalAmount;
        }

        #endregion
    }
}

