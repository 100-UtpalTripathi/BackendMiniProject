namespace CarBookingApplication.Exceptions
{
    public class NotLoggedInException : Exception
    {
        string message;
        public NotLoggedInException()
        {
            message = "User is not logged in!";
        }
        public NotLoggedInException(string name)
        {
            message = name;
        }
        public override string Message => message;
    }
}
