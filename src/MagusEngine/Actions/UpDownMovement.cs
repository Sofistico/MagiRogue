using MagusEngine;
using MagusEngine.Actions.Interfaces;
using MagusEngine.Systems;

namespace MagusEngine.Actions
{
    public class UpDownMovement : IExecuteAction
    {
        private readonly int _zDelta;

        public UpDownMovement(int zDelta)
        {
            _zDelta = zDelta;
        }

        public bool Execute(Universe world)
        {

        }
    }
}
