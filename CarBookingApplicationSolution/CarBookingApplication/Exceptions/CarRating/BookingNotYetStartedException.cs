namespace CarBookingApplication.Exceptions.CarRating
{
    public class BookingNotYetStartedException : Exception
    {
        string message;

        public BookingNotYetStartedException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
