using CarBookingApplication.Interfaces;
using CarBookingApplication.Models.DTOs.QueryDTOs;
using CarBookingApplication.Models;
using CarBookingApplication.Exceptions.Query;
using CarBookingApplication.Exceptions.Customer;

public class QueryService : IQueryService
{
    private readonly IRepository<int, Query> _queryRepository;
    private readonly IRepository<int, Customer> _customerRepository;

    public QueryService(IRepository<int, Query> queryRepository, IRepository<int, Customer> customerRepository)
    {
        _queryRepository = queryRepository;
        _customerRepository = customerRepository;
    }

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

    public async Task<IEnumerable<Query>> GetAllQueriesAsync()
    {
        var queries = await _queryRepository.Get();
        return queries;
    }

    public async Task<Query> GetQueryByIdAsync(int id, int customerId)
    {
        var customer = await _customerRepository.GetByKey(customerId);
        if (customer == null)
        {
            throw new NoSuchCustomerFoundException("Customer not found.");
        }

        var query = await _queryRepository.GetByKey(id);
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

    public async Task<QueryResponseDTO> RespondToQueryAsync(int id, string response)
    {
        var query = await _queryRepository.GetByKey(id);
        if (query == null)
        {
            return new QueryResponseDTO { Success = false, Message = "Query not found." };
        }

        query.Response = response;

        query.Status = "Responded";

        await _queryRepository.Update(query);
        return new QueryResponseDTO { Success = true, Message = "Query responded successfully." };
    }

    public async Task<QueryResponseDTO> CloseQueryAsync(int id)
    {
        var query = await _queryRepository.GetByKey(id);
        if (query == null)
        {
            return new QueryResponseDTO { Success = false, Message = "Query not found." };
        }

        query.Status = "Closed";

        await _queryRepository.Update(query);
        return new QueryResponseDTO { Success = true, Message = "Query closed successfully." };
    }
}
