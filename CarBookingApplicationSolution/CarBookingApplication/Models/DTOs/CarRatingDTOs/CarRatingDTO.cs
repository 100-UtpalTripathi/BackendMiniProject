using System.ComponentModel.DataAnnotations;

namespace CarBookingApplication.Models.DTOs.CarRatingDTOs
{
    public class CarRatingDTO
    {
        [Required(ErrorMessage = "BookingId is required.")]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "CarId is required.")]
        public int CarId { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Review cannot be longer than 500 characters.")]
        public string Review { get; set; }
    }
}


