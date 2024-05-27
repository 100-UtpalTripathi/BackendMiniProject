using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using CarBookingApplication.Models.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmployeeRequestTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Login for users
        /// </summary>
        /// <param name="userLoginDTO">User login details</param>
        /// <returns>Login result</returns>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginReturnDTO>> Login(UserLoginDTO userLoginDTO)
        {
            try
            {
                var result = await _userService.Login(userLoginDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("User not authenticated");
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
        }

        /// <summary>
        /// Register new users
        /// </summary>
        /// <param name="userDTO">User details</param>
        /// <returns>Newly registered user</returns>
        [HttpPost("Register")]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Customer>> Register(CustomerUserDTO userDTO)
        {
            try
            {
                Customer result = await _userService.Register(userDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }

        /// <summary>
        /// Activate user (Admin only)
        /// </summary>
        /// <param name="userActivationDto">User activation details</param>
        /// <returns>Activation result</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ActivateUser([FromBody] UserActivationDTO userActivationDto)
        {
            try
            {
                var result = await _userService.UserActivation(userActivationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }
    }
}
