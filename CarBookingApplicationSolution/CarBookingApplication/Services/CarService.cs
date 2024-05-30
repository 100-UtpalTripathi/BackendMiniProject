using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using CarBookingApplication.Models.DTOs.CarDTOs;
public class CarService : ICarService
{
    #region Private Fields

    private readonly IRepository<int, Car> _carRepository;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CarService"/> class.
    /// </summary>
    /// <param name="carRepository">The car repository.</param>
    public CarService(IRepository<int, Car> carRepository)
    {
        _carRepository = carRepository;
    }

    #endregion


    #region Add-Car
    /// <summary>
    /// Adds a new car asynchronously.
    /// </summary>
    /// <param name="carDto">The car DTO containing car details.</param>
    /// <returns>The result of the operation.</returns>

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

    #endregion

    #region Edit-Car
    /// <summary>
    /// Edits an existing car asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the car to be edited.</param>
    /// <param name="carDto">The car DTO containing updated car details.</param>
    /// <returns>The result of the operation.</returns>

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

    #endregion

    #region Delete-Car
    /// <summary>
    /// Deletes a car asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the car to be deleted.</param>
    /// <returns>The result of the operation.</returns>

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

    #endregion

    #region Get-All-Cars
    /// <summary>
    /// Retrieves all cars asynchronously.
    /// </summary>
    /// <returns>A collection of car DTOs.</returns>

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

    #endregion
}
