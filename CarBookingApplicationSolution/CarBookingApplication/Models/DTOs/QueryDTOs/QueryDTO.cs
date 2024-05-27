using System;
using System.ComponentModel.DataAnnotations;

namespace CarBookingApplication.Models.DTOs.QueryDTOs
{
    public class QueryDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "Subject cannot be longer than 100 characters.")]
        public string Subject { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Message cannot be longer than 1000 characters.")]
        public string Message { get; set; }

        public string Status { get; set; } = "Open";
    }
}

