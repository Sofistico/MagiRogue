using System;

namespace MagusEngine.Exceptions
{
    public class GenericException : Exception
    {
        public GenericException()
        {
        }

        public GenericException(string? message) : base(message)
        {
        }

        public GenericException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
