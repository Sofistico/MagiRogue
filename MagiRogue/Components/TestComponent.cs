﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoRogue;
using GoRogue.GameFramework;
using GoRogue.GameFramework.Components;

namespace MagiRogue.Components
{
    public class TestComponent : IGameObjectComponent
    {
        public TestComponent(IGameObject parent)
        {
            GameLoop.UIManager.MessageLog.Add($"It worked, here are your x and y value: {GameLoop.World.Player.Position.X} {GameLoop.World.Player.Position.Y}");
        }

        public IGameObject Parent { get; set; }
    }
}