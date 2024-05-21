using Microsoft.EntityFrameworkCore;
namespace CarBookingApplication.Contexts
{
    public class CarBookingContext : DbContext
    {
        public CarBookingContext(DbContextOptions<CarBookingContext> options) : base(options)
        {
        }
    }
    
}
