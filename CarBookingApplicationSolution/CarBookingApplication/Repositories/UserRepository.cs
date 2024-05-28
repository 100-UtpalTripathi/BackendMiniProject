using CarBookingApplication.Contexts;
using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using Microsoft.EntityFrameworkCore;
using CarBookingApplication.Exceptions.User;

namespace CarBookingApplication.Repositories
{
    public class UserRepository : IRepository<int, User>
    {
        private CarBookingContext _context;

        public UserRepository(CarBookingContext context)
        {
            _context = context;
        }
        public async Task<User> Add(User item)
        {
            item.Status = "Disabled";
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<User> DeleteByKey(int key)
        {
            var user = await GetByKey(key);
            if (user == null)
            {
                throw new NoSuchUserFoundException("No user found with the given ID");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }


        public async Task<User> GetByKey(int key)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.CustomerId == key);
            return user;
        }

        public async Task<IEnumerable<User>> Get()
        {
            return (await _context.Users.ToListAsync());
        }

        public async Task<User> Update(User item)
        {
            var user = await GetByKey(item.CustomerId);
            if (user != null)
            {
                // Detach the existing entity from the context
                _context.Entry(user).State = EntityState.Detached;

                // Update the entity with the new values
                _context.Update(item);
                await _context.SaveChangesAsync(true);
                return user;
            }
            throw new NoSuchUserFoundException();
        }
    }
}
