using System.ComponentModel.DataAnnotations;

namespace CarBookingApplication.Models.DTOs.UserDTOs
{
    public class ReturnUserActivationDTO
    {
        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, 999, ErrorMessage = "User ID must be between 100 and 999.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [MinLength(1, ErrorMessage = "Status cannot be empty.")]
        public string Status { get; set; } = string.Empty;
    }
}
