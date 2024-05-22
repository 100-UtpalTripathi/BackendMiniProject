namespace CarBookingApplication.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public string Status { get; set; } // e.g., "Available", "Booked", "Maintenance"
        public ICollection<Booking> Bookings { get; set; }
    }
}
