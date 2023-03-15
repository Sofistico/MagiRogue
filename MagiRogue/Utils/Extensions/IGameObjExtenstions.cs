using GoRogue.GameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Utils.Extensions
{
    public static class IGameObjExtenstions
    {
        public static void RemoveThisFromMap(this IGameObject obj)
        {
            Map map = obj.CurrentMap;
            map?.RemoveEntity(obj);
        }
    }
}
