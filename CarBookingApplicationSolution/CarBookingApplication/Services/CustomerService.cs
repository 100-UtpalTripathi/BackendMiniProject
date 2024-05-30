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

namespace CarBookingApplication.Services
{
    public class CustomerService : ICustomerService
    {
        #region Private Fields

        private readonly IRepository<int, Customer> _customerRepository;
        private readonly IRepository<int, Booking> _bookingRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerService"/> class.
        /// </summary>
        /// <param name="customerRepository">The customer repository.</param>
        /// <param name="bookingRepository">The booking repository.</param>
        public CustomerService(IRepository<int, Customer> customerRepository, IRepository<int, Booking> bookingRepository)
        {
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
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
    }
}
