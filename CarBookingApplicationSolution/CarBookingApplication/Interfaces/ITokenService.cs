using CarBookingApplication.Models;

namespace CarBookingApplication.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken(Customer customer);
    }
}
