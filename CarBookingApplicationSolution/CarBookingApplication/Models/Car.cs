using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarBookingApplication.Models
{
    

    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Make is required.")]
        [StringLength(50, ErrorMessage = "Make can't be longer than 50 characters.")]
        public string Make { get; set; }

        [Required(ErrorMessage = "Model is required.")]
        [StringLength(50, ErrorMessage = "Model can't be longer than 50 characters.")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        [Range(1886, 9999, ErrorMessage = "Year must be a valid year.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "City ID is required.")]
        public int CityId { get; set; }

        public City City { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Available|Booked|Maintenance)$", ErrorMessage = "Status must be either 'Available', 'Booked', or 'Maintenance'.")]
        public string Status { get; set; } // e.g., "Available", "Booked", "Maintenance"

        public ICollection<Booking>? Bookings { get; set; }
    }
}
