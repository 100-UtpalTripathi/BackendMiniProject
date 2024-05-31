using CarBookingApplication.Contexts;
using CarBookingApplication.Exceptions.Car;
using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using Microsoft.EntityFrameworkCore;
using CarBookingApplication.Exceptions.CarRating;

namespace CarBookingApplication.Repositories
{
    public class CarRatingRepository : IRepository<int, CarRating>
    {
        private readonly CarBookingContext _context;

        public CarRatingRepository(CarBookingContext context)
        {
            _context = context;
        }
        public async Task<CarRating> Add(CarRating item)
        {
            _context.CarRatings.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<CarRating> DeleteByKey(int key)
        {
            var carRating = await GetByKey(key);
            if (carRating != null)
            {
                _context.Remove(carRating);
                await _context.SaveChangesAsync();
                return carRating;
            }
            throw new NoSuchCarRatingFoundException();
        }

        public async Task<IEnumerable<CarRating>> Get()
        {
            return await _context.CarRatings.ToListAsync();
        }

        public async Task<CarRating> GetByKey(int key)
        {
            var carRating = await _context.CarRatings.FirstOrDefaultAsync(cr => cr.Id == key);
            return carRating;
        }

        public async Task<CarRating> Update(CarRating item)
        {
            var existingCarRating = await GetByKey(item.Id);
            if (existingCarRating != null)
            {
                // Detach the existing entity from the context
                _context.Entry(existingCarRating).State = EntityState.Detached;

                // Attach the provided entity to the context and mark it as modified
                _context.Attach(item);
                _context.Entry(item).State = EntityState.Modified;

                // Save changes to the database
                await _context.SaveChangesAsync();

                return item;
            }
            throw new NoSuchCarRatingFoundException();
        }
    }
}
