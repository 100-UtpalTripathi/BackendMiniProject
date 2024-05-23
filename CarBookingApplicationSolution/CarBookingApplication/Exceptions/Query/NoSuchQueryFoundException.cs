namespace CarBookingApplication.Exceptions.Query
{
    public class NoSuchQueryFoundException : Exception
    {
        string message;

        public NoSuchQueryFoundException()
        {
            message = "No such query found";
        }

        public NoSuchQueryFoundException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
