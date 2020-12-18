using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoRogue;
using GoRogue.GameFramework;
using GoRogue.GameFramework.Components;

namespace MagiRogue.Entities.Components
{
    public class ComponentBase : IGameObjectComponent
    {
        public IGameObject Parent;

        public ComponentBase(IGameObject parent) : base()
        {
            Parent = parent;
        }

        IGameObject IGameObjectComponent.Parent { get; set; }
    }
}