﻿using MagiRogue.GameSys.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Planet.History.HistoryActions
{
    public class MarryAction : BaseHistoryAction
    {
        public MarryAction(int year, WorldTile[,] tiles)
            : base(year, tiles)
        {
        }

        public override void Act(HistoricalFigure figure)
        {
            throw new NotImplementedException();
        }
    }
}