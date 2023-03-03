using GoRogue.Components.ParentAware;
using GoRogue.Pathing;
using MagiRogue.Entities;
using MagiRogue.GameSys;
using MagiRogue.GameSys.Veggies;
using MagiRogue.Utils.Extensions;
using MagiRogue.UI.Windows;
using System;
using System.Linq;
using MagiRogue.Data.Enumerators;
using MagiRogue.Commands;
using MagiRogue.GameSys.Tiles;
using SadRogue.Primitives;

namespace MagiRogue.Components.Ai
{
    public class NeedDrivenAi : IAiComponent
    {
        private Path previousKnowPath;
        private Need? commitedToNeed;

        public IObjectWithComponents? Parent { get; set; }

        public (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog)
        {
            if (Parent.GoRogueComponents.Contains(typeof(NeedCollection)) && Parent is Actor actor)
            {
                var needs = actor.GetComponent<NeedCollection>();

                if (needs is null)
                    return (false, -1);
                if (!needs.GetPriority(out Need need) && commitedToNeed is null)
                    need = needs.FirstOrDefault(i => i.PercentFulfilled <= 25);
                if (commitedToNeed is not null)
                    need = commitedToNeed.Value;

                switch (need.ActionToFulfillNeed)
                {
                    case Data.Enumerators.Actions.Eat:
                        commitedToNeed = ActionManager.FindFood(actor, map);
                        break;

                    case Data.Enumerators.Actions.Sleep:
                        commitedToNeed = ActionManager.Sleep(actor, need);
                        if (commitedToNeed?.TurnCounter == 0)
                        {
                            ClearCommit();
                        }

                        break;

                    case Data.Enumerators.Actions.Drink:
                        var water = map.GetClosestWaterTile(actor.Body.ViewRadius, actor.Position);
                        if (map.DistanceMeasurement.Calculate(actor.Position - water.Position) <= 1) // right next to the water tile or in it
                        {
                            ActionManager.Drink(actor, water.MaterialOfTile, 25, need);
                        }
                        else
                        {
                            commitedToNeed = new Need("Drink Water", false, 0, Actions.GoTo, "Temperance", $"drink {water.ID}")
                            {
                                Objective = water
                            };
                        }
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

                    case Data.Enumerators.Actions.PickUp:
                        break;

                    default:
                        break;
                }

                return (true, 100);
            }
            return (false, -1);
        }

        private void ClearCommit() => commitedToNeed = null;
    }
}
