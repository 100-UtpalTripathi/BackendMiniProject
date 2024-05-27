namespace CarBookingApplication.Models.DTOs.UserDTOs
{
    public class LoginReturnDTO
    {
        public int CustomerID { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
