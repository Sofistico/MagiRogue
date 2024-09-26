namespace Diviner.Enums
{
    public enum WindowTag
    {
        Undefined,
        Main,
        Map,
        Status,
        Wait,
        Look
    }

    public class UndefinedWindowTagException : ApplicationException
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
