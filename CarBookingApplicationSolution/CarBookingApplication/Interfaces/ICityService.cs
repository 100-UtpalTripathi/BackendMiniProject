using CarBookingApplication.Models.DTOs.CityDTOs;
using CarBookingApplication.Models;
namespace CarBookingApplication.Interfaces
{
    public interface ICityService
    {
        Task<CityResponseDTO> AddCityAsync(CityDTO cityDto);
        Task<CityResponseDTO> EditCityAsync(int id, CityDTO cityDto);
        Task<CityResponseDTO> DeleteCityAsync(int id);

        Task<IEnumerable<City>> GetAllCitiesAsync();
    }
}
