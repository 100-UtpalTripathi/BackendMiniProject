namespace CarBookingApplication.Exceptions.Booking
{
    public class NoBookingsFoundException : Exception
    {
        string message;
        public NoBookingsFoundException()
        {
            message = "No Bookings Found!";
        }
        public NoBookingsFoundException(string name)
        {
            message = name;
        }
        public override string Message => message;
    }
}
