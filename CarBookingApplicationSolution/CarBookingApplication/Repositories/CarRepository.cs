using CarBookingApplication.Contexts;
using CarBookingApplication.Exceptions.Car;
using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace CarBookingApplication.Repositories
{
    public class CarRepository : IRepository<int, Car>
    {
        private readonly CarBookingContext _context;

        public CarRepository(CarBookingContext context)
        {
            _context = context;
        }

        public async Task<Car> Add(Car item)
        {
            _context.Cars.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Car> DeleteByKey(int key)
        {
            var car = await GetByKey(key);
            if (car != null)
            {
                _context.Remove(car);
                await _context.SaveChangesAsync();
                return car;
            }
            throw new NoSuchCarFoundException();
        }

        public async Task<IEnumerable<Car>> Get()
        {
            return await _context.Cars.ToListAsync();

        }

        public async Task<Car> GetByKey(int key)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(e => e.Id == key);
            return car;
        }

        public async Task<Car> Update(Car item)
        {
            var car = await GetByKey(item.Id);
            if (car != null)
            {
                _context.Cars.Update(item);
                await _context.SaveChangesAsync();
                return car;
            }
            throw new NoSuchCarFoundException();
        }
    }
}
