using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarBookingApplication.Interfaces;
using CarBookingApplication.Models.DTOs.BookingDTOs;
using CarBookingApplication.Exceptions.User;
using CarBookingApplication.Repositories;
using CarBookingApplication.Models.DTOs.UserDTOs;
using CarBookingApplication.Models;
using CarBookingApplication.Exceptions.Customer;
using CarBookingApplication.Exceptions.Car;
using CarBookingApplication.Models.DTOs.CarRatingDTOs;
using CarBookingApplication.Exceptions.Booking;
using CarBookingApplication.Exceptions.CarRating;

namespace CarBookingApplication.Services
{
    public class CustomerService : ICustomerService
    {
        #region Private Fields

        private readonly IRepository<int, Customer> _customerRepository;
        private readonly IRepository<int, Booking> _bookingRepository;
        private readonly IRepository<int, Car> _carRepository;
        private readonly IRepository<int, CarRating> _carRatingRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerService"/> class.
        /// </summary>
        /// <param name="customerRepository">The customer repository.</param>
        /// <param name="bookingRepository">The booking repository.</param>
        public CustomerService(IRepository<int, Customer> customerRepository, IRepository<int, Booking> bookingRepository, IRepository<int, Car> carRepository, IRepository<int, CarRating> carRatingRepository)
        {
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
            _carRepository = carRepository;
            _carRatingRepository = carRatingRepository;
        }

        #endregion

        #region Get-Customer-Profile
        /// <summary>
        /// Retrieves customer profile asynchronously.
        /// </summary>
        /// <param name="customerId">The identifier of the customer.</param>
        /// <returns>The customer profile DTO.</returns>

        public async Task<CustomerUserDTO> GetCustomerProfileAsync(int customerId)
        {
            var customer = await _customerRepository.GetByKey(customerId);
            if (customer == null)
            {
                throw new NoSuchCustomerFoundException("Customer not found.");
            }
            return MapCustomerToDTO(customer);
        }

        #endregion


        #region Update-Customer-Profile

        /// <summary>
        /// Updates customer profile asynchronously.
        /// </summary>
        /// <param name="customerId">The identifier of the customer.</param>
        /// <param name="customerDTO">The customer DTO containing updated details.</param>
        /// <returns>The updated customer profile DTO.</returns>

        public async Task<CustomerUserDTO> UpdateCustomerProfileAsync(int customerId, CustomerUserDTO customerDTO)
        {
            var existingCustomer = await _customerRepository.GetByKey(customerId);
            if (existingCustomer == null)
            {
                throw new NoSuchCustomerFoundException("Customer not found.");
            }

            // Update customer details
            existingCustomer.Name = customerDTO.Name;
            existingCustomer.DateOfBirth = customerDTO.DateOfBirth;
            existingCustomer.Phone = customerDTO.Phone;
            existingCustomer.Email = customerDTO.Email;

            // Save changes
            await _customerRepository.Update(existingCustomer);

            return MapCustomerToDTO(existingCustomer);
        }

        #endregion

        #region Get-Customer-Bookings

        /// <summary>
        /// Retrieves all bookings of a customer asynchronously.
        /// </summary>
        /// <param name="customerId">The identifier of the customer.</param>
        /// <returns>A collection of bookings.</returns>

        public async Task<IEnumerable<Booking>> GetCustomerBookingsAsync(int customerId)
        {
            var existingCustomer = await _customerRepository.GetByKey(customerId);
            if (existingCustomer == null)
            {
                throw new NoSuchCustomerFoundException("Customer not found.");
            }

            return existingCustomer.Bookings.ToList();
        }

        #endregion

        #region Map-Customer-To-DTO
        /// <summary>
        /// Maps a customer entity to a customer DTO.
        /// </summary>
        /// <param name="customer">The customer entity.</param>
        /// <returns>The mapped customer DTO.</returns>

        private CustomerUserDTO MapCustomerToDTO(Customer customer)
        {
            return new CustomerUserDTO
            {
                Name = customer.Name,
                DateOfBirth = customer.DateOfBirth,
                Phone = customer.Phone,
                Email = customer.Email,
                Role = customer.Role
            };
        }

        #endregion

        #region Rate-Car

        /// <summary>
        /// Adds a rating for a car based on a customer's booking.
        /// </summary>
        /// <param name="customerId">The ID of the customer adding the rating.</param>
        /// <param name="carRatingDTO">The DTO containing the rating information.</param>
        /// <returns>The newly added CarRating object.</returns>

        public async Task<CarRating> AddRatingAsync(int customerId, CarRatingDTO carRatingDTO)
        {
            var customer = await _customerRepository.GetByKey(customerId);
            if (customer == null)
            {
                throw new NoSuchCustomerFoundException("Customer not found!");
            }

            // Check if the customer has any bookings for the specified bookingId
            var booking = customer.Bookings?.FirstOrDefault(b => b.Id == carRatingDTO.BookingId);

            // If booking exists and booking contains the car with given car id check
            if (booking == null || booking.CarId != carRatingDTO.CarId)
            {
                throw new NoSuchBookingFoundException($"Customer {customerId} has no booking with ID {carRatingDTO.BookingId} for car {carRatingDTO.CarId}.");
            }

            if (booking.StartDate > DateTime.UtcNow)
            {
                throw new BookingNotYetStartedException("Booking not yet completed.");
            }

            if (booking.Status == "Cancelled")
            {
                throw new BookingCancelledException("Booking has been cancelled. Can't Rate!");
            }

            var car = await _carRepository.GetByKey(carRatingDTO.CarId);
            if (car == null)
            {
                throw new NoSuchCarFoundException("Car not found!");
            }

            // Check if the customer has already rated the car
            if (car.Ratings != null && car.Ratings.Any(r => r.CustomerId == customerId && r.BookingId == carRatingDTO.BookingId))
            {
                throw new CarRatingAlreadyExistsException("Car already rated by the customer.");
            }

            // Create a new CarRating object
            var carRating = new CarRating
            {
                CarId = carRatingDTO.CarId,
                CustomerId = customerId,
                BookingId = carRatingDTO.BookingId,
                Rating = carRatingDTO.Rating,
                Review = carRatingDTO.Review,
                CreatedDate = DateTime.UtcNow
            };

            // Add the new CarRating to the repository
            var AddedRating = await _carRatingRepository.Add(carRating);

            // Ensure car.Ratings is initialized
            if (car.Ratings == null)
            {
                car.Ratings = new List<CarRating>();
            }

            // Add the new CarRating to the collection
            car.Ratings.Add(AddedRating);

            // Recalculate the average rating for the car
            car.AverageRating = car.Ratings.Average(r => r.Rating);

            // Update the car entity in the repository
            await _carRepository.Update(car);

            return carRating;
        }


        #endregion
    }
}
