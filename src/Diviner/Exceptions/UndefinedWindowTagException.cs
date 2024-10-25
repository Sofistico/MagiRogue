namespace Diviner.Exceptions
{
    public class UndefinedWindowTagException : Exception
    {
        public UndefinedWindowTagException() : base()
        {
        }

        public UndefinedWindowTagException(string? message) : base(message)
        {
        }

        public UndefinedWindowTagException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
