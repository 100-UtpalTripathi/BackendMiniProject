using CarBookingApplication.Interfaces;
using CarBookingApplication.Models.DTOs.QueryDTOs;
using CarBookingApplication.Models;
using CarBookingApplication.Exceptions.Query;
using CarBookingApplication.Exceptions.Customer;

public class QueryService : IQueryService
{
    #region Private Fields

    private readonly IRepository<int, Query> _queryRepository;
    private readonly IRepository<int, Customer> _customerRepository;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryService"/> class.
    /// </summary>
    /// <param name="queryRepository">The query repository.</param>
    /// <param name="customerRepository">The customer repository.</param>
    public QueryService(IRepository<int, Query> queryRepository, IRepository<int, Customer> customerRepository)
    {
        _queryRepository = queryRepository;
        _customerRepository = customerRepository;
    }

    #endregion

    #region Submit-Query
    /// <summary>
    /// Submits a query asynchronously.
    /// </summary>
    /// <param name="queryDto">The query DTO containing query details.</param>
    /// <param name="customerId">The identifier of the customer submitting the query.</param>
    /// <returns>The submitted query.</returns>
    public async Task<Query> SubmitQueryAsync(QueryDTO queryDto, int customerId)
    {
        var customer = await _customerRepository.GetByKey(customerId);
        if (customer == null)
        {
            throw new NoSuchCustomerFoundException("Customer not found.");
        }
        if(customer.Role == "Admin")
        {
            throw new UnauthorizedAccessException("Only customers can submit queries.");
        }

        var query = new Query
        {
            CustomerId = customerId,
            Subject = queryDto.Subject,
            Message = queryDto.Message,
            Status = "Open",
            CreatedDate = DateTime.UtcNow,
        };

        try
        {
            var addedQuery = await _queryRepository.Add(query);
            return addedQuery;
        }
        catch (Exception)
        {
            throw new UnableToSubmitQueryException("Unable to submit query.");
        }
    }

    #endregion

    #region Get-All-Queries
    /// <summary>
    /// Retrieves all open queries asynchronously.
    /// </summary>
    /// <returns>A collection of open queries.</returns>

    public async Task<IEnumerable<Query>> GetAllQueriesAsync(int customerId)
    {
        var customer = await _customerRepository.GetByKey(customerId);
        if (customer == null)
        {
            throw new NoSuchCustomerFoundException("Customer not found.");
        }
        if (customer.Role == "Admin")
        {
            var queries = await _queryRepository.Get();
            return queries.Where(q => q.Status == "Open");
        }
        else
        {
            var queries = await _queryRepository.Get();
            return queries.Where(q => q.CustomerId == customerId);
        }

    }

    #endregion

    #region Get-Query-By-Id
    /// <summary>
    /// Retrieves a query by its ID asynchronously.
    /// </summary>
    /// <param name="queryId">The identifier of the query.</param>
    /// <param name="customerId">The identifier of the customer.</param>
    /// <returns>The query.</returns>

    public async Task<Query> GetQueryByIdAsync(int queryId, int customerId)
    {
        var customer = await _customerRepository.GetByKey(customerId);
        if (customer == null)
        {
            throw new NoSuchCustomerFoundException("Customer not found.");
        }

        var query = await _queryRepository.GetByKey(queryId);
        if (query == null)
        {
            throw new NoSuchQueryFoundException("Query not found.");
        }

        if (customer.Role != "Admin" && query.CustomerId != customerId)
        {
            throw new UnauthorizedAccessException("You can view only your queries!");
        }
        return query;
    }

    #endregion

    #region Respond-To-Query
    /// <summary>
    /// Responds to a query asynchronously.
    /// </summary>
    /// <param name="queryId">The identifier of the query.</param>
    /// <param name="response">The response to the query.</param>
    /// <returns>The response DTO indicating the result of the operation.</returns>

    public async Task<QueryResponseDTO> RespondToQueryAsync(int queryId, string response)
    {
        var query = await _queryRepository.GetByKey(queryId);
        if (query == null)
        {
            return new QueryResponseDTO { Success = false, Message = "Query not found." };
        }

        query.Response = response;

        query.Status = "Responded";

        await _queryRepository.Update(query);
        return new QueryResponseDTO { Success = true, Message = "Query responded successfully." };
    }

    #endregion

    #region Close-Query
    /// <summary>
    /// Closes a query asynchronously.
    /// </summary>
    /// <param name="queryId">The identifier of the query to close.</param>
    /// <returns>The response DTO indicating the result of the operation.</returns>

    public async Task<QueryResponseDTO> CloseQueryAsync(int queryId)
    {
        var query = await _queryRepository.GetByKey(queryId);
        if (query == null)
        {
            return new QueryResponseDTO { Success = false, Message = "Query not found." };
        }

        query.Status = "Closed";

        await _queryRepository.Update(query);
        return new QueryResponseDTO { Success = true, Message = "Query closed successfully." };
    }

    #endregion
}
