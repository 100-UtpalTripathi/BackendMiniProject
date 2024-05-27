using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CarBookingApplication.Models.DTOs.CityDTOs;
using CarBookingApplication.Interfaces;
using CarBookingApplication.Exceptions;
using System;
using CarBookingApplication.Models;
using CarBookingApplication.Exceptions.City;

namespace CarBookingApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CitiesController(ICityService cityService)
        {
            _cityService = cityService;
        }

        /// <summary>
        /// Add a new city (Admin only)
        /// </summary>
        /// <param name="cityDto">City details</param>
        /// <returns>City addition result</returns>
        [HttpPost]
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
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Edit an existing city (Admin only)
        /// </summary>
        /// <param name="id">City ID</param>
        /// <param name="cityDto">Updated city details</param>
        /// <returns>City update result</returns>
        [HttpPut("{id}")]
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
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Delete a city (Admin only)
        /// </summary>
        /// <param name="id">City ID</param>
        /// <returns>City deletion result</returns>
        [HttpDelete("{id}")]
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
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, ex.Message));
            }
        }

        /// <summary>
        /// Get all cities
        /// </summary>
        /// <returns>List of cities</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IList<CityDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<CityDTO>>> GetAllCities()
        {
            try
            {
                var cities = await _cityService.GetAllCitiesAsync();
                return Ok(cities);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, ex.Message));
            }
        }
    }
}
