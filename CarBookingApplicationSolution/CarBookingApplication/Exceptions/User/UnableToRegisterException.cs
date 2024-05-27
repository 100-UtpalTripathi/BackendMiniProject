namespace CarBookingApplication.Exceptions
{
    public class UnableToRegisterException : Exception
    {
        string message;
        public UnableToRegisterException()
        {
            message = "Unable to register!";
        }
        public UnableToRegisterException(string name)
        {
            message = name;
        }
        public override string Message => message;  
    }
}
