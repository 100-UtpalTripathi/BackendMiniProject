using CarBookingApplication.Interfaces;
using CarBookingApplication.Models.DTOs.BookingDTOs;
using CarBookingApplication.Models.DTOs.UserDTOs;
using CarBookingApplication.Models;

namespace CarBookingApplication.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<int, Customer> _customerRepository;
        public CustomerService(IRepository<int, Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }
        Task<IEnumerable<BookingDTO>> ICustomerService.GetCustomerBookingsAsync(int customerId)
        {
            throw new NotImplementedException();
        }

        Task<CustomerUserDTO> ICustomerService.GetCustomerProfileAsync(int customerId)
        {
            throw new NotImplementedException();
        }

        Task<CustomerUserDTO> ICustomerService.UpdateCustomerProfileAsync(int customerId, CustomerUserDTO customerDTO)
        {
            throw new NotImplementedException();
        }
    }
}
