using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CarBookingApplication.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Customer), nameof(ValidateAge))]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(15, ErrorMessage = "Phone number can't be longer than 15 characters.")]
        public string Phone { get; set; }

        public string Image { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [StringLength(50, ErrorMessage = "Role can't be longer than 50 characters.")]
        public string Role { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Query>? Queries { get; set; }


        // Custom validation method to check age, If the validation passes,
        // return ValidationResult.Success;
        // if the validation fails, return a new ValidationResult with an error message.
        public static ValidationResult ValidateAge(DateTime dateOfBirth, ValidationContext context)
        {
            var age = DateTime.Now.Year - dateOfBirth.Year;
            if (dateOfBirth > DateTime.Now.AddYears(-age)) 
                age--;

            return age >= 18? ValidationResult.Success: new ValidationResult("Customer must be at least 18 years old.");
        }
    }

}
