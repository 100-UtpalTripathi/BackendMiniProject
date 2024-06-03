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

        [Required(ErrorMessage = "Transmission is required.")]
        [RegularExpression("^(Manual|Automatic)$", ErrorMessage = "Transmission must be either 'Manual' or 'Automatic'.")]
        public string Transmission { get; set; }

        [Required(ErrorMessage = "Number of seats is required.")]
        [Range(1, 20, ErrorMessage = "Number of seats must be between 1 and 20.")]
        public int NumberOfSeats { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [RegularExpression("^(Small|Medium|Large|SUV|Premium)$", ErrorMessage = "Category must be one of the following: 'Small', 'Medium', 'Large', 'SUV', 'Premium'.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Price per day is required.")]
        public double PricePerDay { get; set; }

        public ICollection<Booking>? Bookings { get; set; }


        public double? AverageRating { get; set; } // Stores the average rating
        public ICollection<CarRating>? Ratings { get; set; } // List of ratings
    }
}
