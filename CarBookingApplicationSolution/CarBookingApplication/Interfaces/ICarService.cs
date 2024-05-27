using CarBookingApplication.Models.DTOs.CarDTOs;
namespace CarBookingApplication.Interfaces
{
    public interface ICarService
    {
        Task<CarResponseDTO> AddCarAsync(CarDTO carDto);
        Task<CarResponseDTO> EditCarAsync(int id, CarDTO carDto);
        Task<CarResponseDTO> DeleteCarAsync(int id);
        Task<IEnumerable<CarDTO>> GetAllCarsAsync();
    }
}
