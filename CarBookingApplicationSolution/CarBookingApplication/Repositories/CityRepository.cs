using CarBookingApplication.Models;
using CarBookingApplication.Interfaces;
using CarBookingApplication.Exceptions.City;
using CarBookingApplication.Contexts;
using Microsoft.EntityFrameworkCore;


namespace CarBookingApplication.Repositories
{
    public class CityRepository : IRepository<int, City>
    {
        private readonly CarBookingContext _context;

        public CityRepository(CarBookingContext context)
        {
            _context = context;
        }

        public async Task<City> Add(City item)
        {
            _context.Cities.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<City> DeleteByKey(int key)
        {
            var City = await GetByKey(key);
            if (City != null)
            {
                _context.Remove(City);
                await _context.SaveChangesAsync(true);
                return City;
            }
            throw new NoSuchCityFoundException();
        }

        public Task<City> GetByKey(int key)
        {
            var City = _context.Cities.FirstOrDefaultAsync(e => e.Id == key);
            return City;
        }

        public async Task<IEnumerable<City>> Get()
        {
            var Cities = await _context.Cities.ToListAsync();
            return Cities;

        }

        public async Task<City> Update(City item)
        {
            var city = await GetByKey(item.Id);
            if (city != null)
            {
                // Detach the existing entity from the context
                _context.Entry(city).State = EntityState.Detached;

                // Update the entity with the new values
                _context.Update(item);
                await _context.SaveChangesAsync();
                return item;
            }
            throw new NoSuchCityFoundException();
        }


    }
}
