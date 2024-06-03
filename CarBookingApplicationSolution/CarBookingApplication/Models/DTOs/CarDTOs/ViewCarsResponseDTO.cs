using System.ComponentModel.DataAnnotations;

namespace CarBookingApplication.Models.DTOs.CarDTOs
{
    public class ViewCarsResponseDTO
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int CityId { get; set; }
        public string Status { get; set; }
        public string Transmission { get; set; }
        public int NumberOfSeats { get; set; }
        public string Category { get; set; }

        public double? AverageRating { get; set; } = 0;

        public double PricePerDay { get; set; }
    }
}
