using GoRogue.Components.ParentAware;
using MagiRogue.Entities;
using MagiRogue.GameSys;
using MagiRogue.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Components.Ai
{
    public class NeedDrivenAi : IAiComponent
    {
        public IObjectWithComponents? Parent { get; set; }

        public (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog)
        {
            if (Parent.GoRogueComponents.Contains(typeof(NeedCollection)) && Parent is Actor actor)
            {
                var needs = actor.GetComponent<NeedCollection>();

                if (needs is null)
                    return (false, -1);

                var lowestNeed = needs.FirstOrDefault(i => i.PercentFulfilled <= 25);

                switch (lowestNeed.ActionToFulfillNeed)
                {
                    case Data.Enumerators.Actions.Eat:
                        var whatToEat = actor.GetAnatomy().WhatToEat();
                        //var foodItem = map.FindTypeOfFood(whatToEat);
                        break;

                    case Data.Enumerators.Actions.Sleep:
                        break;

                    case Data.Enumerators.Actions.Drink:
                        break;

                    case Data.Enumerators.Actions.Fun:
                        break;

                    case Data.Enumerators.Actions.Train:
                        break;

                    case Data.Enumerators.Actions.Pray:
                        break;

                    case Data.Enumerators.Actions.Study:
                        break;

                    case Data.Enumerators.Actions.Teach:
                        break;

                    case Data.Enumerators.Actions.Craft:
                        break;

                    case Data.Enumerators.Actions.Fight:
                        break;

                    case Data.Enumerators.Actions.Bully:
                        break;

                    default:
                        break;
                }

                return (true, 100);
            }
            return (false, -1);
        }
    }
}
