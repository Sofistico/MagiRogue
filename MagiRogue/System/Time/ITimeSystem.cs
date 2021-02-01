using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.Entities;

namespace MagiRogue.System.Time
{
    public interface ITimeSystem
    {
        void RegisterEntity(ITimeNode node);

        void DeRegisterEntity(ITimeNode node);

        ITimeNode NextNode();
    }
}