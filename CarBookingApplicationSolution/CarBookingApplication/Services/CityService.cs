using CarBookingApplication.Interfaces;
using CarBookingApplication.Models.DTOs.CityDTOs;
using CarBookingApplication.Models;

public class CityService : ICityService
{
    private readonly IRepository<int, City> _cityRepository;

    public CityService(IRepository<int, City> cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<CityResponseDTO> AddCityAsync(CityDTO cityDto)
    {
        var city = new City
        {
            Name = cityDto.Name,
            State = cityDto.State,
            Country = cityDto.Country
        };

        await _cityRepository.Add(city);

        return new CityResponseDTO { Success = true, Message = "City added successfully." };
    }

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

        await _cityRepository.Update(city);
        return new CityResponseDTO { Success = true, Message = "City updated successfully." };
    }

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

    public async Task<IEnumerable<CityDTO>> GetAllCitiesAsync()
    {
        var cities = await _cityRepository.Get();
        return cities.Select(c => new CityDTO
        {
            Name = c.Name,
            State = c.State,
            Country = c.Country
        });
    }
}
