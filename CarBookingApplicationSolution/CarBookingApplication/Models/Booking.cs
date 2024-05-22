namespace CarBookingApplication.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; } // New field for discount
        public decimal FinalAmount { get; set; } // New field for final amount after discount
        public string Status { get; set; } // e.g., "Confirmed", "Cancelled", "Completed"
    }
}
