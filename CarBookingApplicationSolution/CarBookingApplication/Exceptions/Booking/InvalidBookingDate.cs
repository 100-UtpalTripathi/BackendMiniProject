namespace CarBookingApplication.Exceptions.Booking
{
    public class InvalidBookingDate : Exception
    {
        string message;

        public InvalidBookingDate(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
