using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CarBookingApplication.Models.DTOs.BookingDTOs
{
    public class DateNotInPastAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime >= DateTime.Today)
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult(ErrorMessage ?? "Date cannot be in the past.");
            }
            return new ValidationResult("Invalid date format.");
        }
    }

    public class EndDateAfterStartDateAttribute : ValidationAttribute
    {
        private readonly string _startDatePropertyName;

        public EndDateAfterStartDateAttribute(string startDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var endDate = (DateTime)value;

            var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
            if (startDateProperty == null)
            {
                return new ValidationResult($"Unknown property: {_startDatePropertyName}");
            }

            var startDateValue = startDateProperty.GetValue(validationContext.ObjectInstance);
            if (startDateValue == null)
            {
                return new ValidationResult("Start date is required.");
            }

            var startDate = (DateTime)startDateValue;

            if (endDate <= startDate)
            {
                return new ValidationResult(ErrorMessage ?? "End Date must be after Start Date.");
            }

            return ValidationResult.Success;
        }
    }

    public class BookingDTO
    {
        [Required(ErrorMessage = "Car ID is required.")]
        public int CarId { get; set; }

        [Required(ErrorMessage = "Booking Date is required.")]
        [DataType(DataType.Date)]
        [DateNotInPast(ErrorMessage = "Booking Date cannot be in the past.")]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        [DateNotInPast(ErrorMessage = "Start Date cannot be in the past.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date)]
        [EndDateAfterStartDate("StartDate", ErrorMessage = "End Date must be after Start Date.")]
        public DateTime EndDate { get; set; }
    }
}
