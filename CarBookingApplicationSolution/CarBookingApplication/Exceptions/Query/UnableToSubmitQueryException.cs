namespace CarBookingApplication.Exceptions.Query
{
    public class UnableToSubmitQueryException : Exception
    {
        string message;

        public UnableToSubmitQueryException()
        {
            message = "Unable to submit query";
        }

        public UnableToSubmitQueryException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
