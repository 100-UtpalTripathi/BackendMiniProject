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

namespace CarBookingApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Get all bookings (Admin only)
        /// </summary>
        /// <returns>List of bookings</returns>
        [Authorize(Roles = "Admin")]
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
                return Ok(bookings);
            }
            catch (NotLoggedInException ex)
            {
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

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
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

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
        public async Task<ActionResult<BookingResponseDTO>> CancelBooking(int BookingId)
        {
            try
            {
                var customerId = User.FindFirstValue("eid");
                if (customerId == null)
                {
                    throw new NotLoggedInException("User is not logged in.");
                }

                var result = await _bookingService.CancelBookingAsync(BookingId, int.Parse(customerId));
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
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Book a car
        /// </summary>
        /// <param name="bookingRequest">Booking request details</param>
        /// <returns>Booking result</returns>
        [Authorize]
        [HttpPost("book")]
        [ProducesResponseType(typeof(BookingDTO), StatusCodes.Status200OK)]
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
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, ex.Message));
            }
        }



        [HttpPost("{bookingId}/extend")]
        [ProducesResponseType(typeof(BookingResponseDTO), 200)]
        [ProducesResponseType(typeof(BookingResponseDTO), 400)]
        [ProducesResponseType(typeof(BookingResponseDTO), 404)]
        [ProducesResponseType(typeof(BookingResponseDTO), 500)]
        public async Task<IActionResult> ExtendBooking(int bookingId, [FromBody] ExtendBookingDTO extendBookingDTO)
        {
            try
            {
                var customerId = User.FindFirstValue("eid");
                if (customerId == null)
                {
                    throw new NotLoggedInException("User is not logged in.");
                }

                var result = await _bookingService.ExtendBookingAsync(bookingId, extendBookingDTO.NewEndDate, int.Parse(customerId));
                return Ok(result);
            }
            catch (NoSuchBookingFoundException ex)
            {
                return NotFound(new BookingResponseDTO { Success = false, Message = ex.Message });
            }
            catch (NoSuchCustomerFoundException ex)
            {
                return BadRequest(new BookingResponseDTO { Success = false, Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new BookingResponseDTO { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new BookingResponseDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new BookingResponseDTO { Success = false, Message = "An unexpected error occurred." });
            }
        }
    }
}
