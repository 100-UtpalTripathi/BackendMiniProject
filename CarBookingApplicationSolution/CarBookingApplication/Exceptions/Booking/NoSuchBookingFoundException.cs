namespace CarBookingApplication.Exceptions.Booking
{
    public class NoSuchBookingFoundException : Exception
    {
        string message;
        public NoSuchBookingFoundException()
        {
            message = "No Booking found with given ID!";
        }
        public NoSuchBookingFoundException(string name)
        {
            message = name;
        }
        public override string Message => message;
    }
}
