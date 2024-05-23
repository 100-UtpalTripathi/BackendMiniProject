using CarBookingApplication.Contexts;
using CarBookingApplication.Exceptions.Query;
using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace CarBookingApplication.Repositories
{
    public class QueryRepository : IRepository<int, Query>
    {
        private readonly CarBookingContext _context;

        public QueryRepository(CarBookingContext context)
        {
            _context = context;
        }

        public async Task<Query> Add(Query item)
        {
            _context.Queries.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Query> DeleteByKey(int key)
        {
            var Query = await GetByKey(key);
            if (Query != null)
            {
                _context.Remove(Query);
                await _context.SaveChangesAsync(true);
                return Query;
            }
            throw new NoSuchQueryFoundException();
        }

        public Task<Query> GetByKey(int key)
        {
            var Query = _context.Queries.FirstOrDefaultAsync(e => e.Id == key);
            return Query;
        }

        public async Task<IEnumerable<Query>> Get()
        {
            var Queries = await _context.Queries.ToListAsync();
            return Queries;

        }

        public async Task<Query> Update(Query item)
        {
            var Query = await GetByKey(item.Id);
            if (Query != null)
            {
                _context.Queries.Update(item);
                await _context.SaveChangesAsync(true);
                return Query;
            }
            throw new NoSuchQueryFoundException();
        }
    }
}
