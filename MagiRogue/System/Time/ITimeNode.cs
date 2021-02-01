using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Time
{
    public interface ITimeNode
    {
        public long Tick { get; }
    }
}