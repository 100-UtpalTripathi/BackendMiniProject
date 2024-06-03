namespace CarBookingApplication.Exceptions.CarRating
{
    public class CarRatingAlreadyExistsException : Exception
    {
        string message;

        public CarRatingAlreadyExistsException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
