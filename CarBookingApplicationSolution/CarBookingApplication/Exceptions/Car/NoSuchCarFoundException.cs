namespace CarBookingApplication.Exceptions.Car
{
    public class NoSuchCarFoundException : Exception
    {
        string message;
        public NoSuchCarFoundException()
        {
            message = "No Car found with given ID!";
        }
        public NoSuchCarFoundException(string name)
        {
            message = name;
        }
        public override string Message => message;
    }
}
