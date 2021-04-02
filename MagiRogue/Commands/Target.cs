﻿using GoRogue;
using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Tiles;
using MagiRogue.UI;
using Microsoft.Xna.Framework;
using SadConsole.Input;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Commands
{
    /// <summary>
    /// Defines an target system.
    /// </summary>
    public class Target : ITarget
    {
        public Coord TargetCoord { get; set; }
        public Entity Cursor { get; set; }

        public Target(Coord targetCoord)
        {
            TargetCoord = targetCoord;
            Color targetColor = new Color(255, 0, 0);

            Cursor = new Actor(targetColor, Color.Transparent, 'X', (int)MapLayer.PLAYER, TargetCoord)
            {
                Name = "Target Cursor",
                IsWalkable = true,
                CanBeKilled = false,
                CanBeAttacked = false,
                CanInteract = false
            };

            var frameCell = Cursor.Animation.CreateFrame()[0];
            frameCell.Foreground = Color.Transparent;
            frameCell.Background = Color.Transparent;
            frameCell.Glyph = 'X';

            Cursor.Animation.AnimationDuration = 2.0f;
            Cursor.Animation.Repeat = true;

            Cursor.Animation.Start();
        }

        public T TargetEntity<T>(Entity entity) where T : Entity
        {
            throw new NotImplementedException();
        }

        public T TargetTile<T>(TileBase tile) where T : TileBase
        {
            throw new NotImplementedException();
        }
    }
}