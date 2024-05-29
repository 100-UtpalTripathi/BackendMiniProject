using CarBookingApplication.Contexts;
using CarBookingApplication.Interfaces;
using CarBookingApplication.Models;
using Microsoft.EntityFrameworkCore;
using CarBookingApplication.Exceptions.Customer;

namespace CarBookingApplication.Repositories
{
    public class CustomerRepository : IRepository<int, Customer>
    {
        private readonly CarBookingContext _context;
        public CustomerRepository(CarBookingContext context)
        {
            _context = context;
        }
        public async Task<Customer> Add(Customer item)
        {
            _context.Customers.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Customer> DeleteByKey(int key)
        {
            var Customer = await GetByKey(key);
            if (Customer != null)
            {
                _context.Remove(Customer);
                await _context.SaveChangesAsync(true);
                return Customer;
            }
            throw new NoSuchCustomerFoundException();
        }

        public Task<Customer> GetByKey(int key)
        {
            var Customer = _context.Customers.FirstOrDefaultAsync(e => e.Id == key);
            return Customer;
        }

        public async Task<IEnumerable<Customer>> Get()
        {
            var Customers = await _context.Customers.ToListAsync();
            return Customers;

        }

        public async Task<Customer> Update(Customer item)
        {
            var existingCustomer = await GetByKey(item.Id);
            if (existingCustomer != null)
            {
                // Detach the existing entity from the context
                _context.Entry(existingCustomer).State = EntityState.Detached;

                // Update the entity with the new values
                _context.Customers.Update(item);
                await _context.SaveChangesAsync(true);
                return item;
            }
            throw new NoSuchCustomerFoundException();
        }

    }
}
