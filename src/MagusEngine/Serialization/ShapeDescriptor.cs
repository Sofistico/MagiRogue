﻿using Arquimedes.Interfaces;

namespace MagusEngine.Serialization
{
    public class ShapeDescriptor : IJsonKey
    {
        public string Id { get; set; }
        public string[] Name { get; set; }
        public string[] Adjectives { get; set; }
        public string Tile { get; set; }
    }
}