using MagusEngine.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagusEngine.Bus.MapBus
{
    public class ChangeControlledEntitiy
    {
        public MagiEntity ControlledEntitiy { get; set; }

        public ChangeControlledEntitiy(MagiEntity controlledEntitiy)
        {
            ControlledEntitiy = controlledEntitiy;
        }
    }
}
