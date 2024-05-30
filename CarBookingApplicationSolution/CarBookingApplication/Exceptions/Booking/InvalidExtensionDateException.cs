namespace CarBookingApplication.Exceptions.Booking
{
    public class InvalidExtensionDateException : Exception
    {
        string message;

        public InvalidExtensionDateException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
