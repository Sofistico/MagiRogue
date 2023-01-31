using GoRogue.Components.ParentAware;
using MagiRogue.Entities;
using MagiRogue.GameSys;
using MagiRogue.GameSys.Time;
using MagiRogue.UI.Windows;
using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Components.Ai
{
    public class BasicAi : IAiComponent
    {
        private readonly MagiEntity _entity;
        public IObjectWithComponents? Parent { get; set; }

        public BasicAi(MagiEntity entity)
        {
            _entity = entity;
        }

        public (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog)
        {
            bool rng = Mrn.OneIn(10);
            if (rng)
            {
                messageLog.Add($"The {_entity.Name} waits doing nothing...");
                return (true, TimeHelper.Wait);
            }
            else
                return (false, TimeHelper.AiFailed);
        }
    }
}