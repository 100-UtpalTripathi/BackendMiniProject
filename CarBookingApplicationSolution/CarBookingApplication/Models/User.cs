using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarBookingApplication.Models
{
    public class User
    {
        [Key]
        public int CustomerId { get; set; }
        public byte[] Password { get; set; }
        public byte[] PasswordHashKey { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public string Status { get; set; }
    }
}
