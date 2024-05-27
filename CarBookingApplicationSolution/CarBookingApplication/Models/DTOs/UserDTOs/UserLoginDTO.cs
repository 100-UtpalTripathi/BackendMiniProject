using System.ComponentModel.DataAnnotations;

namespace CarBookingApplication.Models
{
    public class UserLoginDTO
    {
        [Required(ErrorMessage = "User id cannot be empty")]
        [Range(1, 999, ErrorMessage = "Invalid entry for employee ID")]
        public int UserId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password cannot be empty")]
        [MinLength(6, ErrorMessage = "Password has to be minimum 6 characters long")]
        public string Password { get; set; } = string.Empty;
    }
}
