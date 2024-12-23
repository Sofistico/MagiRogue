﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RuleFor
    {
        Null,
        Marriage,
        HaveChild,
        TrainAbility,
        GenerateMagicalResources,
        GetAFriend,
        LearnDiscoveriesKnowToSite,
        WanderAndSettle,
        ResearchWork,
        BuildATower,
        BuildACity,
        CreateNewCiv,
        MythGenericStuff
    }
}
