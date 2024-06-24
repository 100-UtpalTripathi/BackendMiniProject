using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CarBookingApplication.Models.DTOs.CityDTOs;
using CarBookingApplication.Interfaces;
using CarBookingApplication.Exceptions;
using System;
using CarBookingApplication.Models;
using CarBookingApplication.Exceptions.City;
using Microsoft.AspNetCore.Cors;

namespace CarBookingApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("MyCors")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;
        private readonly ILogger<CitiesController> _logger;
        public CitiesController(ICityService cityService, ILogger<CitiesController> logger)
        {
            _cityService = cityService;
            _logger = logger;
        }

        #region Add-City
        /// <summary>
        /// Add a new city (Admin only)
        /// </summary>
        /// <param name="cityDto">City details</param>
        /// <returns>City addition result</returns>
        [HttpPost("add")]
        [ProducesResponseType(typeof(CityResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CityResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CityResponseDTO>> AddCity([FromBody] CityDTO cityDto)
        {
            try
            {
                var result = await _cityService.AddCityAsync(cityDto);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, ex.Message));
            }
        }

        #endregion

        #region Edit-City
        /// <summary>
        /// Edit an existing city (Admin only)
        /// </summary>
        /// <param name="id">City ID</param>
        /// <param name="cityDto">Updated city details</param>
        /// <returns>City update result</returns>
        [HttpPut("edit/{id}")]
        [ProducesResponseType(typeof(CityResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CityResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CityResponseDTO>> EditCity(int id, [FromBody] CityDTO cityDto)
        {
            try
            {
                var result = await _cityService.EditCityAsync(id, cityDto);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (NoSuchCityFoundException ex)
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


        #region Delete-City

        /// <summary>
        /// Delete a city (Admin only)
        /// </summary>
        /// <param name="id">City ID</param>
        /// <returns>City deletion result</returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(typeof(CityResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CityResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CityResponseDTO>> DeleteCity(int id)
        {
            try
            {
                var result = await _cityService.DeleteCityAsync(id);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (NoSuchCityFoundException ex)
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


        #region Get all cities

        /// <summary>
        /// Get all cities
        /// </summary>
        /// <returns>List of cities</returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(IList<City>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<City>>> GetAllCities()
        {
            try
            {
                var cities = await _cityService.GetAllCitiesAsync();

                if(!cities.Any())
                {
                    return NoContent();
                }
                return Ok(cities);
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
