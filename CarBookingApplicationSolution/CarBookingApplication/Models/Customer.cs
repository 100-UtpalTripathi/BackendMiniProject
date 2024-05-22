namespace CarBookingApplication.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Image { get; set; } = string.Empty;
        public string Email { get; set; }
        public string Role { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Query> Queries { get; set; }
    }

}
