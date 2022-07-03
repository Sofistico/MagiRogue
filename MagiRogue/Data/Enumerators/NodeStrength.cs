using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Enumerators
{
    public enum NodeStrength
    {
        Fading = -3,
        Feeble = 1,
        Weak = 2,
        Normal = 0,
        Strong = 3,
        Powerful = 4,
        DemigodLike = 6,
        Godlike = 10
    }
}