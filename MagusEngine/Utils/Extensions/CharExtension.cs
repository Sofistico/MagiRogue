using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagusEngine.Utils.Extensions
{
    public static class CharExtension
    {
        public static char GetCharHotkey(int count)
        {
            return (char)(96 + count);
        }
    }
}
