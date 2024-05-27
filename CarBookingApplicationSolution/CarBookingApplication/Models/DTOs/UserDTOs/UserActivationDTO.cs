using System.ComponentModel.DataAnnotations;


namespace CarBookingApplication.Models.DTOs.UserDTOs
{
    public class UserActivationDTO
    {
        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, 999, ErrorMessage = "User ID must be between 1 and 999.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "IsActive status is required.")]
        public bool IsActive { get; set; }
    }
}
