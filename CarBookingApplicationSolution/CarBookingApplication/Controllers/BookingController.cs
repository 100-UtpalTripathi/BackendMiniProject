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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            try
            {
                var customerId = User.FindFirstValue("eid");
                if (customerId == null)
                {

                    throw new NotLoggedInException("User is not logged in.");
                }

                var booking = await _bookingService.GetBookingByIdAsync(id, customerId);
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

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var result = await _bookingService.CancelBookingAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("{id}/issue")]
        public async Task<IActionResult> HandleBookingIssue(int id, [FromBody] string issueDetails)
        {
            var result = await _bookingService.HandleBookingIssueAsync(id, issueDetails);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }

}
