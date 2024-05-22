namespace CarBookingApplication.Models
{
    public class Query
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Response { get; set; }
        public string Status { get; set; } // e.g., "Open", "Closed"
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
