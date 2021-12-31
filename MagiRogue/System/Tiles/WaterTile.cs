﻿using MagiRogue.Entities;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Tiles
{
    public class WaterTile : TileBase
    {
        public bool IsSea { get; set; }
        public int SwinDifficulty { get; private set; } = 2;

        public WaterTile(Color foregroud, Color background, int glyph, int layer, Point position,
            bool isSea,
            string idOfMaterial = "h20", bool blocksMove = false, bool isTransparent = true,
            string name = "Water")
            : base(foregroud, background, glyph, layer, position, idOfMaterial,
                  blocksMove, isTransparent, name)
        {
            IsSea = isSea;
            if (IsSea)
                SwinDifficulty += 2;
        }

        public bool CanSwinThere(Actor actor)
        {
            if (actor.Abilities[((int)AbilityName.Swin)].Score > SwinDifficulty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}