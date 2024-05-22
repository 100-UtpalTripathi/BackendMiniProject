
    using System;
    using System.ComponentModel.DataAnnotations;

    namespace CarBookingApplication.Models
    {
        public class Booking
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Car ID is required.")]
            public int CarId { get; set; }
            public Car Car { get; set; }

            [Required(ErrorMessage = "Customer ID is required.")]
            public int CustomerId { get; set; }
            public Customer Customer { get; set; }

            [Required(ErrorMessage = "Booking date is required.")]
            public DateTime BookingDate { get; set; }

            [Required(ErrorMessage = "Start date is required.")]
            [DataType(DataType.Date)]
            public DateTime StartDate { get; set; }

            [Required(ErrorMessage = "End date is required.")]
            [DataType(DataType.Date)]
            public DateTime EndDate { get; set; }

            [Required(ErrorMessage = "Total amount is required.")]
            [Range(0, double.MaxValue, ErrorMessage = "Total amount must be a positive value.")]
            public decimal TotalAmount { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Discount amount must be a positive value.")]
            public decimal DiscountAmount { get; set; } // New field for discount

            [Required(ErrorMessage = "Final amount is required.")]
            [Range(0, double.MaxValue, ErrorMessage = "Final amount must be a positive value.")]
            public decimal FinalAmount { get; set; } // New field for final amount after discount

            [Required(ErrorMessage = "Status is required.")]
            [RegularExpression("^(Confirmed|Cancelled|Completed)$", ErrorMessage = "Status must be either 'Confirmed', 'Cancelled', or 'Completed'.")]
            public string Status { get; set; } // e.g., "Confirmed", "Cancelled", "Completed"
        }
    }


