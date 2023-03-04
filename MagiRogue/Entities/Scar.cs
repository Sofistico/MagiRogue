﻿using MagiRogue.Data.Enumerators;

namespace MagiRogue.Entities
{
    public record struct Scar
    {
        public string Desc { get; set; }
        public int Severity { get; set; }
        public ScarType Type { get; init; }

        public Scar(string desc,
            int severity,
            ScarType type)
        {
            Desc = desc;
            Severity = severity;
            Type = type;
        }
    }
}