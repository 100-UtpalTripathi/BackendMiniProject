namespace CarBookingApplication.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CarRating
    {
        public int Id { get; set; }

        [Required]
        public int CarId { get; set; }

        public Car Car { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public int BookingId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Review cannot be longer than 500 characters.")]
        public string Review { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }


}
