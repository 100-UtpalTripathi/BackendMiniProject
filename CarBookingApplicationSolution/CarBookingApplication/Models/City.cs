using System.ComponentModel.DataAnnotations;

namespace CarBookingApplication.Models
{
    public class City
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "City name is required.")]
        [StringLength(100, ErrorMessage = "City name can't be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [StringLength(100, ErrorMessage = "State can't be longer than 100 characters.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(100, ErrorMessage = "Country can't be longer than 100 characters.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Pincode is required.")]
        [StringLength(6, ErrorMessage = "Pincode can't be longer than 6 characters.")]
        public string Pincode { get; set; }

        public ICollection<Car>? Cars { get; set; }
    }
}
