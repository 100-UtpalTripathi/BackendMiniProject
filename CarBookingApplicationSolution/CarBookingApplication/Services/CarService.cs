using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using CarBookingApplication.Models.DTOs.CarDTOs;
public class CarService : ICarService
{
    private readonly IRepository<int, Car> _carRepository;

    public CarService(IRepository<int, Car> carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<CarResponseDTO> AddCarAsync(CarDTO carDto)
    {
        var car = new Car
        {
            Make = carDto.Make,
            Model = carDto.Model,
            Year = carDto.Year,
            CityId = carDto.CityId,
            Status = carDto.Status,
            Transmission = carDto.Transmission,
            NumberOfSeats = carDto.NumberOfSeats,
            Category = carDto.Category
        };

        await _carRepository.Add(car);
        return new CarResponseDTO { Success = true, Message = "Car added successfully." };
    }

    public async Task<CarResponseDTO> EditCarAsync(int id, CarDTO carDto)
    {
        var car = await _carRepository.GetByKey(id);
        if (car == null)
        {
            return new CarResponseDTO { Success = false, Message = "Car not found." };
        }

        car.Make = carDto.Make;
        car.Model = carDto.Model;
        car.Year = carDto.Year;
        car.CityId = carDto.CityId;
        car.Status = carDto.Status;
        car.Transmission = carDto.Transmission;
        car.NumberOfSeats = carDto.NumberOfSeats;
        car.Category = carDto.Category;

        await _carRepository.Update(car);
        return new CarResponseDTO { Success = true, Message = "Car updated successfully." };
    }

    public async Task<CarResponseDTO> DeleteCarAsync(int id)
    {
        var car = await _carRepository.GetByKey(id);
        if (car == null)
        {
            return new CarResponseDTO { Success = false, Message = "Car not found." };
        }

        await _carRepository.DeleteByKey(id);
        return new CarResponseDTO { Success = true, Message = "Car deleted successfully." };
    }

    public async Task<IEnumerable<CarDTO>> GetAllCarsAsync()
    {
        var cars = await _carRepository.Get();
        return cars.Select(c => new CarDTO
        {
            Make = c.Make,
            Model = c.Model,
            Year = c.Year,
            CityId = c.CityId,
            Status = c.Status,
            Transmission = c.Transmission,
            NumberOfSeats = c.NumberOfSeats,
            Category = c.Category
        });
    }
}
