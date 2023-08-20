using Arquimedes.Enumerators;
using MagusEngine.Core;
using System;
using System.Collections.Generic;

namespace MagusEngine.Systems
{
    public class BasicTile
    {
        public string? TileId { get; set; }
        public List<Trait> Traits { get; set; } = new();

        public Tile Copy()
        {
            throw new NotImplementedException();
        }

        internal bool HasAnyTrait()
        {
            throw new NotImplementedException();
        }
    }
}