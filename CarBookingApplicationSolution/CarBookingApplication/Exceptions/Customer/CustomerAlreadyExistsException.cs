namespace CarBookingApplication.Exceptions.Customer
{
    public class CustomerAlreadyExistsException : Exception
    {
        string message;

        public CustomerAlreadyExistsException()
        {
            message = "Customer already exists";
        }

        public CustomerAlreadyExistsException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
