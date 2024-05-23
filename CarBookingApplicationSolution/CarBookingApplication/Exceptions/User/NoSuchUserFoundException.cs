namespace CarBookingApplication.Exceptions.User
{
    public class NoSuchUserFoundException : Exception
    {
        string message;
        public NoSuchUserFoundException()
        {
            message = "No User found with given ID!";
        }
        public NoSuchUserFoundException(string name)
        {
            message = name;
        }
        public override string Message => message;
    }
}

