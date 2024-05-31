namespace CarBookingApplication.Exceptions.CarRating
{
    public class BookingCancelledException : Exception
    {
        string message;

        public BookingCancelledException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
