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

namespace CarBookingApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
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
    }
}
