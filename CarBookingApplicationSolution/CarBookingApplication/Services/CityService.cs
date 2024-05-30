using CarBookingApplication.Interfaces;
using CarBookingApplication.Models.DTOs.CityDTOs;
using CarBookingApplication.Models;

public class CityService : ICityService
{
    #region Private Fields

    private readonly IRepository<int, City> _cityRepository;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CityService"/> class.
    /// </summary>
    /// <param name="cityRepository">The city repository.</param>
    public CityService(IRepository<int, City> cityRepository)
    {
        _cityRepository = cityRepository;
    }

    #endregion


    #region Add-City
    /// <summary>
    /// Adds a new city asynchronously.
    /// </summary>
    /// <param name="cityDto">The city DTO containing city details.</param>
    /// <returns>The result of the operation.</returns>

    public async Task<CityResponseDTO> AddCityAsync(CityDTO cityDto)
    {
        var city = new City
        {
            Name = cityDto.Name,
            State = cityDto.State,
            Country = cityDto.Country,
            Pincode = cityDto.Pincode
        };

        await _cityRepository.Add(city);

        return new CityResponseDTO { Success = true, Message = "City added successfully." };
    }

    #endregion

    #region Edit-City
    /// <summary>
    /// Edits an existing city asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the city to be edited.</param>
    /// <param name="cityDto">The city DTO containing updated city details.</param>
    /// <returns>The result of the operation.</returns>

    public async Task<CityResponseDTO> EditCityAsync(int id, CityDTO cityDto)
    {
        var city = await _cityRepository.GetByKey(id);
        if (city == null)
        {
            return new CityResponseDTO { Success = false, Message = "City not found." };
        }

        city.Name = cityDto.Name;
        city.State = cityDto.State;
        city.Country = cityDto.Country;
        city.Pincode = cityDto.Pincode;

        await _cityRepository.Update(city);
        return new CityResponseDTO { Success = true, Message = "City updated successfully." };
    }

    #endregion

    #region Delete-City
    /// <summary>
    /// Deletes a city asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the city to be deleted.</param>
    /// <returns>The result of the operation.</returns>

    public async Task<CityResponseDTO> DeleteCityAsync(int id)
    {
        var city = await _cityRepository.GetByKey(id);
        if (city == null)
        {
            return new CityResponseDTO { Success = false, Message = "City not found." };
        }

        await _cityRepository.DeleteByKey(id);
        return new CityResponseDTO { Success = true, Message = "City deleted successfully." };
    }

    #endregion

    #region Get-All-Cities
    /// <summary>
    /// Retrieves all cities asynchronously.
    /// </summary>
    /// <returns>A collection of cities.</returns>

    public async Task<IEnumerable<City>> GetAllCitiesAsync()
    {
        var cities = await _cityRepository.Get();
        return cities;
    }

    #endregion
}
