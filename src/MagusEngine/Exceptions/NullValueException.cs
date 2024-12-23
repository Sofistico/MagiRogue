using System;

namespace MagusEngine.Exceptions
{
    public class NullValueException : Exception
    {
        public NullValueException() { }

        public NullValueException(string? message, Exception? innerException)
            : base(message, innerException) { }

        public NullValueException(string? nameofValue)
            : base($"Value {nameofValue} is null.") { }
    }
}
