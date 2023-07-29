using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.UI.Enums
{
    public enum WindowTag
    {
        Undefined,
        Main,
        Map,
        Status,
        Wait
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
