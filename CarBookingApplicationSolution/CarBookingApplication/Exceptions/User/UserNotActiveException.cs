namespace CarBookingApplication.Exceptions
{
    public class UserNotActiveException : Exception
    {
        string message;
        public UserNotActiveException()
        {
            message = "User is not active!";
        }
        public UserNotActiveException(string name)
        {
            message = name;
        }
        public override string Message => message;
    }
}
