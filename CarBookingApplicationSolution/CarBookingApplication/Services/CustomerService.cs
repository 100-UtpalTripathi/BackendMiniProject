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
        private readonly IRepository<int, Customer> _customerRepository;
        private readonly IRepository<int, Booking> _bookingRepository;

        public CustomerService(IRepository<int, Customer> customerRepository, IRepository<int, Booking> bookingRepository)
        {
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<CustomerUserDTO> GetCustomerProfileAsync(int customerId)
        {
            var customer = await _customerRepository.GetByKey(customerId);
            if (customer == null)
            {
                throw new NoSuchCustomerFoundException("Customer not found.");
            }
            return MapCustomerToDTO(customer);
        }

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

        public async Task<IEnumerable<Booking>> GetCustomerBookingsAsync(int customerId)
        {
            var existingCustomer = await _customerRepository.GetByKey(customerId);
            if (existingCustomer == null)
            {
                throw new NoSuchCustomerFoundException("Customer not found.");
            }

            return existingCustomer.Bookings.ToList();
        }

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
    }
}
