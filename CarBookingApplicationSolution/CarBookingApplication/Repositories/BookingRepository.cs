using CarBookingApplication.Contexts;
using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using Microsoft.EntityFrameworkCore;
using CarBookingApplication.Exceptions.Booking;


namespace CarBookingApplication.Repositories
{
    public class BookingRepository : IRepository<int, Booking>
    {
        private readonly CarBookingContext _context;

        public BookingRepository(CarBookingContext context)
        {
            _context = context;
        }

        public async Task<Booking> Add(Booking item)
        {
            _context.Bookings.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Booking> DeleteByKey(int key)
        {
            var booking = await GetByKey(key);
            if (booking != null)
            {
                _context.Remove(booking);
                await _context.SaveChangesAsync();
                return booking;
            }
            throw new NoSuchBookingFoundException();
        }

        public async Task<IEnumerable<Booking>> Get()
        {  
            return await _context.Bookings.ToListAsync();
            
        }

        public async Task<Booking> GetByKey(int key)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(e => e.Id == key);
            return booking;
        }

        public async Task<Booking> Update(Booking item)
        {
            var booking = await GetByKey(item.Id);
            if (booking != null)
            {
                _context.Bookings.Update(item);
                await _context.SaveChangesAsync();
                return booking;
            }
            throw new NoSuchBookingFoundException();
        }
    }
}
