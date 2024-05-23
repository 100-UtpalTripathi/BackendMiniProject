namespace CarBookingApplication.Exceptions.Customer
{
    public class NoSuchCustomerFoundException : Exception
    {
        string message;
        public NoSuchCustomerFoundException()
        {
            message = "No Customer found with given ID!";
        }
        public NoSuchCustomerFoundException(string name)
        {
            message = name;
        }
        public override string Message => message;
    }
}
