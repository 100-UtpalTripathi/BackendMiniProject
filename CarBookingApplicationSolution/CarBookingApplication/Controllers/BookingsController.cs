using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CarBookingApplication.Models.DTOs.BookingDTOs;
using CarBookingApplication.Exceptions.Booking;
using CarBookingApplication.Exceptions;
using CarBookingApplication.Models.DTOs.QueryDTOs;
using System.Security.Claims;
using CarBookingApplication.Exceptions.Customer;
using CarBookingApplication.Exceptions.Car;
using Microsoft.AspNetCore.Cors;

namespace CarBookingApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("MyCors")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }


        #region Get all bookings
        /// <summary>
        /// Get all bookings
        /// </summary>
        /// <returns>List of bookings</returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IList<Booking>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<Booking>>> GetAllBookings()
        {
            try
            {
                var customerId = User.FindFirstValue("eid");
                if (customerId == null)
                {
                    throw new NotLoggedInException("User is not logged in.");
                }

                var bookings = await _bookingService.GetAllBookingsAsync(int.Parse(customerId));

                if (bookings.Any())
                    return Ok(bookings);
                else
                    return Ok("No Bookings found!");
            }
            catch (NotLoggedInException ex)
            {
                _logger.LogError(ex.Message);
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        #endregion

        #region Get Bookings by Customer ID
        /// <summary>
        /// Get booking by ID
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Booking details</returns>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Booking), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Booking>> GetBookingById(int id)
        {
            try
            {
                var customerId = User.FindFirstValue("eid");
                if (customerId == null)
                {
                    throw new NotLoggedInException("User is not logged in.");
                }

                var booking = await _bookingService.GetBookingByIdAsync(id, int.Parse(customerId));
                return Ok(booking);
            }
            catch (NotLoggedInException ex)
            {
                _logger.LogError(ex.Message);
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch(NoSuchBookingFoundException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
            catch(UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion

        #region Cancel Booking
        /// <summary>
        /// Cancel a booking
        /// </summary>
        /// <param name="BookingId">Booking ID</param>
        /// <returns>Cancellation result</returns>
        [Authorize]
        [HttpPut("{id}/cancel")]
        [ProducesResponseType(typeof(BookingResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BookingResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookingResponseDTO>> CancelBooking(int id)
        {
            try
            {
                var customerId = User.FindFirstValue("eid");
                if (customerId == null)
                {
                    throw new NotLoggedInException("User is not logged in.");
                }

                var result = await _bookingService.CancelBookingAsync(id, int.Parse(customerId));
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (NotLoggedInException ex)
            {
                _logger.LogError(ex.Message);
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch(NoSuchBookingFoundException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new BookingResponseDTO { Success = false, Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
        #endregion

        #region Add-Booking

        /// <summary>
        /// Book a car
        /// </summary>
        /// <param name="bookingRequest">Booking request details</param>
        /// <returns>Booking result</returns>
        [Authorize]
        [HttpPost("book")]
        [ProducesResponseType(typeof(BookingDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookingResponseDTO>> BookCar([FromBody] BookingDTO bookingRequest)
        {
            try
            {
                var customerId = User.FindFirstValue("eid");
                if (customerId == null)
                {
                    throw new NotLoggedInException("User is not logged in.");
                }

                var result = await _bookingService.BookCarAsync(int.Parse(customerId), bookingRequest);
                return Ok(result);
            }
            catch (NotLoggedInException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status401Unauthorized, new ErrorModel(401, ex.Message));
            }
            catch (NoSuchCarFoundException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status404NotFound, new ErrorModel(404, ex.Message));
            }
            catch (CarNotAvailableForBookingException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status409Conflict, new ErrorModel(409, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, "An unexpected error occurred while processing the booking."));
            }
        }

        #endregion

        #region Extend Bookings

        [HttpPut("{Id}/extend")]
        [ProducesResponseType(typeof(BookingResponseDTO), 200)]
        [ProducesResponseType(typeof(BookingResponseDTO), 400)]
        [ProducesResponseType(typeof(BookingResponseDTO), 404)]
        [ProducesResponseType(typeof(BookingResponseDTO), 500)]
        public async Task<IActionResult> ExtendBooking(int Id, [FromBody] ExtendBookingDTO extendBookingDTO)
        {
            try
            {
                var customerId = User.FindFirstValue("eid");
                if (customerId == null)
                {
                    throw new NotLoggedInException("User is not logged in.");
                }

                var result = await _bookingService.ExtendBookingAsync(Id, extendBookingDTO.NewEndDate, int.Parse(customerId));
                return Ok(result);
            }
            catch (NoSuchBookingFoundException ex)
            {
                _logger.LogError(ex.Message);
                return NotFound(new BookingResponseDTO { Success = false, Message = ex.Message });
            }
            catch (NoSuchCustomerFoundException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new BookingResponseDTO { Success = false, Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(403, new BookingResponseDTO { Success = false, Message = ex.Message });
            }
            catch (InvalidExtensionDateException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new BookingResponseDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new BookingResponseDTO { Success = false, Message = "An unexpected error occurred." });
            }
        }

        #endregion
    }
}
