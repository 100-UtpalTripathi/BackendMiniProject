namespace CarBookingApplication.Exceptions
{
    public class UnauthorizedUserException : Exception
    {
        string message;
        public UnauthorizedUserException()
        {
            message = "Unauthorized User!";
        }
        public UnauthorizedUserException(string name)
        {
            message = name;
        }
        public override string Message => message;
    }
}
