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
            return await _context.Bookings
                         .Include(b => b.Car)
                         .ToListAsync();

        }

        public async Task<Booking> GetByKey(int key)
        {
            var booking = await _context.Bookings.Include(b => b.Car).FirstOrDefaultAsync(e => e.Id == key);
            return booking;
        }

        public async Task<Booking> Update(Booking item)
        {
            var existingBooking = await GetByKey(item.Id);
            if (existingBooking != null)
            {
                // Detach the existing entity from the context
                _context.Entry(existingBooking).State = EntityState.Detached;

                // Attach the provided entity to the context and mark it as modified
                _context.Attach(item);
                _context.Entry(item).State = EntityState.Modified;

                // Save changes to the database
                await _context.SaveChangesAsync();

                return item;
            }
            throw new NoSuchBookingFoundException();
        }

    }
}
