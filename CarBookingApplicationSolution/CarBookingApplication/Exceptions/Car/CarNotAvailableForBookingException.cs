namespace CarBookingApplication.Exceptions.Car
{
    public class CarNotAvailableForBookingException : Exception
    {
        string message;

        public CarNotAvailableForBookingException()
        {
            message = "Car is not available for booking!";
        }

        public CarNotAvailableForBookingException(string name)
        {
            message = name;
        }

        public override string Message => message;
    }
}
