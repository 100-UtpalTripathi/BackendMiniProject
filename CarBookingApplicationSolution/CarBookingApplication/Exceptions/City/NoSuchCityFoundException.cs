namespace CarBookingApplication.Exceptions.City
{
    public class NoSuchCityFoundException : Exception
    {
        string message;

        public NoSuchCityFoundException()
        {
            message = "No such city found";
        }

        public NoSuchCityFoundException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
