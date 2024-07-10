using CarBookingApplication.Models.DTOs.QueryDTOs;
using CarBookingApplication.Models;
namespace CarBookingApplication.Interfaces
{
    public interface IQueryService
    {
        Task<Query> SubmitQueryAsync(QueryDTO queryDto, int customerId);
        Task<IEnumerable<Query>> GetAllQueriesAsync(int customerId);
        Task<Query> GetQueryByIdAsync(int id, int customerId);
        Task<QueryResponseDTO> RespondToQueryAsync(int id, string response);
        Task<QueryResponseDTO> CloseQueryAsync(int id);
    }
}
