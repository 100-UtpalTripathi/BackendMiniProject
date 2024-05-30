using CarBookingApplication.Interfaces;
using CarBookingApplication.Models.DTOs.BookingDTOs;
using CarBookingApplication.Models.DTOs.UserDTOs;
using CarBookingApplication.Exceptions.User;
using CarBookingApplication.Exceptions.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CarBookingApplication.Models;
using CarBookingApplication.Exceptions;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarBookingApplication.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;
        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        /// <summary>
        /// Get customer profile
        /// </summary>
        /// <returns>Customer profile</returns>
        [HttpGet("profile/view")]
        [ProducesResponseType(typeof(CustomerUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerUserDTO>> GetCustomerProfile()
        {
            try
            {
                var customerId = User.FindFirstValue("eid");
                if (customerId == null)
                {
                    throw new NotLoggedInException("User is not logged in.");
                }
                var customer = await _customerService.GetCustomerProfileAsync(int.Parse(customerId));
                return Ok(customer);
            }
            catch (NotLoggedInException ex)
            {
                _logger.LogError(ex.Message);
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch (NoSuchCustomerFoundException ex)
            {
                _logger.LogError(ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Update customer profile
        /// </summary>
        /// <param name="customerDTO">Customer data transfer object</param>
        /// <returns>Updated customer profile</returns>
        [HttpPut("profile/edit")]
        [ProducesResponseType(typeof(CustomerUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerUserDTO>> UpdateCustomerProfile([FromBody] CustomerUserDTO customerDTO)
        {
            try
            {
                var customerId = User.FindFirstValue("eid");
                if (customerId == null)
                {
                    throw new NotLoggedInException("User is not logged in.");
                }
                var updatedCustomer = await _customerService.UpdateCustomerProfileAsync(int.Parse(customerId), customerDTO);
                return Ok(updatedCustomer);
            }
            catch (NotLoggedInException ex)
            {
                _logger.LogError(ex.Message);
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch (NoSuchCustomerFoundException ex)
            {
                _logger.LogError(ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Get bookings of the customer
        /// </summary>
        /// <returns>List of bookings</returns>
        [HttpGet("bookings")]
        [ProducesResponseType(typeof(IEnumerable<Booking>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Booking>>> GetCustomerBookings()
        {
            try
            {
                var customerId = User.FindFirstValue("eid");
                if (customerId == null)
                {
                    throw new NotLoggedInException("User is not logged in.");
                }
                var bookings = await _customerService.GetCustomerBookingsAsync(int.Parse(customerId));

                if(!bookings.Any())
                {
                    return NotFound(new ErrorModel(404, "No bookings found."));
                }
                return Ok(bookings);
            }
            catch (NotLoggedInException ex)
            {
                _logger.LogError(ex.Message);
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch (NoSuchCustomerFoundException ex)
            {
                _logger.LogError(ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
    }
}
