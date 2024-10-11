namespace Shared.Exceptions
{
    public class OrderExecutionException : Exception
    { 

        public OrderExecutionException(string? message) : base(message)
        {
        }

        public OrderExecutionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
