using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data
{
    /// <summary>
    /// This class will deal with the serialization of the spells and it's effects, will use a tag like CDDA
    /// does to determine the effects
    /// </summary>
    public class SpellTemplate
    {
    }

    public enum EffectTypes
    {
        DAMAGE,
        HASTE,
        MAGESIGHT,
        SEVER,
        TELEPORT
    }
}