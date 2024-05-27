using Azure.Core;
using CarBookingApplication.Exceptions;
using CarBookingApplication.Interfaces;
using CarBookingApplication.Models.DTOs.QueryDTOs;
using CarBookingApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CarBookingApplication.Exceptions.Query;

namespace CarBookingApplication.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class QueriesController : ControllerBase
    {
        private readonly IQueryService _queryService;

        public QueriesController(IQueryService queryService)
        {
            _queryService = queryService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitQuery([FromBody] QueryDTO queryDto)
        {
            try
            {
                var customerId = User.FindFirstValue("eid");
                if (customerId == null)
                {

                    throw new NotLoggedInException("User is not logged in.");
                }

                var result = await _queryService.SubmitQueryAsync(queryDto, int.Parse(customerId));
                return Ok(result);
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllQueries()
        {
            var queries = await _queryService.GetAllQueriesAsync();
            return Ok(queries);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQueryById(int id)
        {
            try
            {
                var customerId = User.FindFirstValue("eid");
                if (customerId == null)
                {
                    throw new NotLoggedInException("User is not logged in.");
                }
                var query = await _queryService.GetQueryByIdAsync(id, int.Parse(customerId));
                return Ok(query);
            }
            catch (NoSuchQueryFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
            
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/respond")]
        public async Task<IActionResult> RespondToQuery(int id, [FromBody] string response)
        {
            try
            {
                var result = await _queryService.RespondToQueryAsync(id, response);
                return Ok(result);
            }
            catch (NoSuchQueryFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/close")]
        public async Task<ActionResult<QueryResponseDTO>> CloseQuery(int id)
        {
            try
            {
                var result = await _queryService.CloseQueryAsync(id);
                return Ok(result);
            }
            catch (NoSuchQueryFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }
    }

}
