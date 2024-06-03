using CarBookingApplication.Models.DTOs.CarDTOs;
using CarBookingApplication.Models;
namespace CarBookingApplication.Interfaces
{
    public interface ICarService
    {
        Task<CarResponseDTO> AddCarAsync(CarDTO carDto);
        Task<CarResponseDTO> EditCarAsync(int id, CarDTO carDto);
        Task<CarResponseDTO> DeleteCarAsync(int id);
        Task<IEnumerable<ViewCarsResponseDTO>> GetAllCarsAsync();

    
    }
}
