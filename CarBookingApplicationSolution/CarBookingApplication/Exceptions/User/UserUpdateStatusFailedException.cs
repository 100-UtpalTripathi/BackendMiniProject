namespace CarBookingApplication.Exceptions.User
{
    public class UserUpdateStatusFailedException : Exception
    {
        string message;
        public UserUpdateStatusFailedException()
        {
            message = "User Update Status Failed!";
        }
        public UserUpdateStatusFailedException(string name)
        {
            message = name;
        }
        public override string Message => message;
    }
}
