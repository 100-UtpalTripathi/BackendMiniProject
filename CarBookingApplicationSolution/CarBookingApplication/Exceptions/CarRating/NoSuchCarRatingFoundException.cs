namespace CarBookingApplication.Exceptions.CarRating
{
    public class NoSuchCarRatingFoundException : Exception
    {
        string message;

        public NoSuchCarRatingFoundException()
        {
                message = "No such car rating found.";
        }

        public NoSuchCarRatingFoundException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
