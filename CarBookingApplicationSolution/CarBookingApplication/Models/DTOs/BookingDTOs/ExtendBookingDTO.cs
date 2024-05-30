using System.ComponentModel.DataAnnotations;

namespace CarBookingApplication.Models.DTOs.BookingDTOs
{
    public class ExtendBookingDTO
    {
        [Required(ErrorMessage = "New end date is required.")]
        [DataType(DataType.Date)]
        public DateTime NewEndDate { get; set; }
    }
}
