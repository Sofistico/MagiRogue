using System;
using Game = SadConsole.Game;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Test
{
    public class MockGame : IDisposable
    {
        public void Dispose()
        {
            Game.Instance.Exit();
            GC.SuppressFinalize(this);
        }

        public MockGame()
        {
            Game.Create(1, 1);

            Game.Instance.RunOneFrame();
        }
    }
}