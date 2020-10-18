using Microsoft.Xna.Framework;
using MagiRogue.Utils;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Resources;

namespace MagiRogue.Entities.Items
{
    class IronBar : Item
    {
        public IronBar() : base(Color.Gray, Color.Transparent, "Iron Bar", '_', 10)
        {
        }
    }
}