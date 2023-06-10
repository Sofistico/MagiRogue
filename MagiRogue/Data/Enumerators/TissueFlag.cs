﻿using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TissueFlag
    {
        /// <summary>
        /// Thickens with fat
        /// </summary>
        ThickensOnEnergyStore,
        /// <summary>
        /// thickens with strength training
        /// </summary>
        ThickensOnStrTraining,
        /// <summary>
        /// Is a muscule, has nerves that can be severed and lost movement and or senses
        /// </summary>
        Muscular,
        /// <summary>
        /// Gives structure to the body part, if severed or broken part loses movement
        /// </summary>
        Structural,
        /// <summary>
        /// Is an anchor for other stuff tbd
        /// </summary>
        ConnectiveTissueAnchor,
        /// <summary>
        /// The tissue scars
        /// </summary>
        Scars,
        /// <summary>
        /// has artery that when struck duplicates bleeding potentially
        /// </summary>
        Artery
    }
}
