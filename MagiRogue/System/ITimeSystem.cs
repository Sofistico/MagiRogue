using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.Entities;

namespace MagiRogue.System
{
    public interface ITimeSystem
    {
        void RegisterEntity(Entity entity);

        void DeRegisterEntity(Entity entity);

        void ProgressTime();
    }
}