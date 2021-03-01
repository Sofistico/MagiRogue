﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace MagiRogue.Entities.Data
{
    [DataContract]
    [Serializable]
    public class ActorTemplate
    {
        [DataMember]
        public Color Foreground { get; set; }
        [DataMember]
        public Color Background { get; set; }
        [DataMember]
        public int Glyph { get; set; }
        [DataMember]
        public Stat Stats { get; set; }
        [DataMember]
        public Anatomy Anatomy { get; set; }

        public ActorTemplate(Color foreground, Color background, int glyph, Stat stats, Anatomy anatomy)
        {
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
            Stats = stats;
            Anatomy = anatomy;
        }
    }
}