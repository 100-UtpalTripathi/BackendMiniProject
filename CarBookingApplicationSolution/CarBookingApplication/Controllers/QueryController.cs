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
        private readonly ILogger<QueriesController> _logger;
        public QueriesController(IQueryService queryService, ILogger<QueriesController> logger)
        {
            _queryService = queryService;
            _logger = logger;
        }


        #region Submit-Query
        /// <summary>
        /// Submit a new query (Authenticated users only)
        /// </summary>
        /// <param name="queryDto">Query details</param>
        /// <returns>Query submission result</returns>
        [Authorize]
        [HttpPost("add")]
        [ProducesResponseType(typeof(QueryResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
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
                _logger.LogError(ex.Message);
                return Unauthorized(new ErrorModel(401, ex.Message));
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

        #region Get-All-Queries

        /// <summary>
        /// Get all queries (Admin only)
        /// </summary>
        /// <returns>List of queries</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("get/all")]
        [ProducesResponseType(typeof(IList<Query>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllQueries()
        {
            try
            {
                var queries = await _queryService.GetAllQueriesAsync();
                return Ok(queries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorModel(500, ex.Message));
            }
        }

        #endregion

        #region Get-Query-By-ID

        /// <summary>
        /// Get a query by its ID (Authenticated users only)
        /// </summary>
        /// <param name="id">Query ID</param>
        /// <returns>Query details</returns>
        [Authorize]
        [HttpGet("get/{id}")]
        [ProducesResponseType(typeof(Query), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
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
            catch (NotLoggedInException ex)
            {
                _logger.LogError(ex.Message);
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch (NoSuchQueryFoundException ex)
            {
                _logger.LogError(ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
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

        #region Respond-To-Query

        /// <summary>
        /// Respond to a query (Admin only)
        /// </summary>
        /// <param name="id">Query ID</param>
        /// <param name="response">Response message</param>
        /// <returns>Query response</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/respond")]
        [ProducesResponseType(typeof(QueryResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RespondToQuery(int id, [FromBody] string response)
        {
            try
            {
                var result = await _queryService.RespondToQueryAsync(id, response);
                return Ok(result);
            }
            catch (NoSuchQueryFoundException ex)
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

        #region Close-Query

        /// <summary>
        /// Close a query (Admin only)
        /// </summary>
        /// <param name="id">Query ID</param>
        /// <returns>Query closure result</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/close")]
        [ProducesResponseType(typeof(QueryResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QueryResponseDTO>> CloseQuery(int id)
        {
            try
            {
                var result = await _queryService.CloseQueryAsync(id);
                return Ok(result);
            }
            catch (NoSuchQueryFoundException ex)
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
    }

}
