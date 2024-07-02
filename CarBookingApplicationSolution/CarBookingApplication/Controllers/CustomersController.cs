using CarBookingApplication.Interfaces;
using CarBookingApplication.Models.DTOs.UserDTOs;
using CarBookingApplication.Exceptions.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarBookingApplication.Models;
using CarBookingApplication.Exceptions;
using System.Security.Claims;
using CarBookingApplication.Exceptions.Booking;
using CarBookingApplication.Exceptions.Car;
using CarBookingApplication.Models.DTOs.CarRatingDTOs;
using CarBookingApplication.Exceptions.CarRating;
using CarBookingApplication.Models.DTOs.CarDTOs;
using Microsoft.AspNetCore.Cors;

namespace CarBookingApplication.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("MyCors")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        #region Private Fields
        private readonly ICustomerService _customerService;
        private readonly ICarService _carService;
        private readonly ILogger<CustomersController> _logger;
        #endregion

        #region Constructor
        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger, ICarService carService)
        {
            _customerService = customerService;
            _logger = logger;
            _carService = carService;
        }

        #endregion

        #region Customer-Profile
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

        #endregion

        #region Edit-Customer-Profile

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

        #endregion


        #region Customer-Bookings

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

        #endregion


        #region Rate-Car

        /// <summary>
        /// Rate a car based on a customer's booking.
        /// </summary>
        /// <param name="carRatingDTO">The DTO containing the rating information.</param>
        /// <returns>The newly added CarRating object.</returns>
        [Authorize]
        [HttpPost("rate-car")]
        [ProducesResponseType(typeof(CarRating), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CarRating>> RateCar([FromBody] CarRatingDTO carRatingDTO)
        {
            try
            {
                var customerId = User.FindFirstValue("eid");
                if (customerId == null)
                {
                    throw new NotLoggedInException("User is not logged in.");
                }

                var carRating = await _customerService.AddRatingAsync(int.Parse(customerId), carRatingDTO);
                return Ok(carRating);
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
            catch (NoSuchBookingFoundException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (NoSuchCarFoundException ex)
            {
                _logger.LogError(ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch(BookingNotYetStartedException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }

            catch(BookingCancelledException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch(CarRatingAlreadyExistsException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        #endregion

        #region View-Cars

        [HttpGet("view-cars")]
        [ProducesResponseType(typeof(IEnumerable<ViewCarsResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ViewCarsResponseDTO>>> ViewCars()
        {
            try
            {
                var cars = await _carService.GetAllCarsAsync();
                

                if (!cars.Any())
                {
                    return NotFound(new ErrorModel(404, "No cars found."));
                }
                cars = cars.Where(car => car.Status == "Available").ToList();

                return Ok(cars);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion


    }
}
