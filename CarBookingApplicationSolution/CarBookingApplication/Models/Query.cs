using System.ComponentModel.DataAnnotations;

namespace CarBookingApplication.Models
{
    public class Query
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(100, ErrorMessage = "Subject can't be longer than 100 characters.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(1000, ErrorMessage = "Message can't be longer than 1000 characters.")]
        public string Message { get; set; }

        [StringLength(1000, ErrorMessage = "Response can't be longer than 1000 characters.")]
        public string? Response { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Open|Closed)$", ErrorMessage = "Status must be either 'Open' or 'Closed'.")]
        public string Status { get; set; } // e.g., "Open", "Closed"

        [Required(ErrorMessage = "Created Date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format.")]
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; } = null;

    }
}
