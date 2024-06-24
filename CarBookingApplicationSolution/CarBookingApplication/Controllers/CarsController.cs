using CarBookingApplication.Interfaces;
using CarBookingApplication.Models.DTOs.CarDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CarBookingApplication.Exceptions;
using System;
using CarBookingApplication.Models;
using CarBookingApplication.Exceptions.Car;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;


namespace CarBookingApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("MyCors")]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly ILogger<CarsController> _logger;
        public CarsController(ICarService carService, ILogger<CarsController> logger)
        {
            _carService = carService;
            _logger = logger;
        }


        #region Add-Car
        /// <summary>
        /// Add a new car (Admin only)
        /// </summary>
        /// <param name="carDto">Car details</param>
        /// <returns>Car addition result</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        [ProducesResponseType(typeof(CarResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CarResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CarResponseDTO>> AddCar([FromBody] CarDTO carDto)
        {
            try
            {
                var result = await _carService.AddCarAsync(carDto);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, ex.Message));
            }
        }

        #endregion


        #region Edit-Car

        /// <summary>
        /// Edit an existing car (Admin only)
        /// </summary>
        /// <param name="id">Car ID</param>
        /// <param name="carDto">Updated car details</param>
        /// <returns>Car update result</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("edit/{id}")]
        [ProducesResponseType(typeof(CarResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CarResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CarResponseDTO>> EditCar(int id, [FromBody] CarDTO carDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _carService.EditCarAsync(id, carDto);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (NoSuchCarFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, ex.Message));
            }
        }

        #endregion


        #region Delete-Car
        /// <summary>
        /// Delete a car (Admin only)
        /// </summary>
        /// <param name="id">Car ID</param>
        /// <returns>Car deletion result</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(typeof(CarResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CarResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CarResponseDTO>> DeleteCar(int id)
        {
            try
            {
                var result = await _carService.DeleteCarAsync(id);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (NoSuchCarFoundException ex)
            {
                _logger.LogError(ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, ex.Message));
            }
        }

        #endregion


        #region Get All Cars

        /// <summary>
        /// Get all cars
        /// </summary>
        /// <returns>List of cars</returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<ViewCarsResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ViewCarsResponseDTO>>> GetAllCars()
        {
            try
            {
                var cars = await _carService.GetAllCarsAsync();
                if (!cars.Any())
                {
                    return Ok("No cars available at this time.");
                }
                return Ok(cars);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, ex.Message));
            }
        }

        #endregion

    }
}
