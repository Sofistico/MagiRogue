using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Services;
using MagusEngine.Systems;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Serialization.EntitySerialization
{
    public class BodyPlanCollection
    {
        public List<BodyPlan> BodyPlans { get; set; }

        public BodyPlanCollection()
        {
            BodyPlans = new();
        }

        public List<BodyPart> ExecuteAllBodyPlans(Race race = null!)
        {
            var list = new List<BodyPart>();
            foreach (var plan in BodyPlans)
            {
                list.AddRange(plan.ReturnBodyParts(race));
            }
            return list;
        }
    }

    public class BodyPlan
    {
        private readonly ConcurrentDictionary<string, Tissue> _raceTissue = new();
        private readonly List<TissueLayeringTemplate> _raceTissueLayering = new();
        private readonly object _lockObj = new();

        public string? Id { get; set; }
        public List<string> BodyParts { get; set; }

        //public List<Tissue> Tissues { get; set; }
        public int Numbers { get; set; }

        public BodyPlan()
        {
            BodyParts = new();
        }

        public List<BodyPart> ReturnBodyParts(Race race = null!)
        {
            List<BodyPart> returnParts = new();
            string[] tissues = race?.Tissues!;
            if (tissues?.Length > 0)
            {
                for (int i = 0; i < tissues.Length; i++)
                {
                    var tissueGroup = DataManager.QueryTissuePlanInData(tissues[i]);
                    tissueGroup?.Tissues?.ForEach(i => _raceTissue?.TryAdd(i.Id, i));
                    if (tissueGroup?.TissueLayering?.Count > 0)
                    {
                        lock (_lockObj)
                        {
                            _raceTissueLayering?.AddRange(tissueGroup.TissueLayering);
                        }
                    }
                }
            }

            foreach (string bp in BodyParts)
            {
                Limb? limb = DataManager.QueryLimbInData(bp);
                Organ? organ = DataManager.QueryOrganInData(bp);
                if (limb is not null)
                {
                    returnParts.Add(limb);
                }
                if (organ is not null)
                {
                    returnParts.Add(organ);
                }
                if (limb is null && organ is null)
                    throw new ApplicationException($"Coudn't find a valid body part! bodypart id: {bp}");

                if (_raceTissueLayering.Count > 0 && !_raceTissue.IsEmpty)
                {
                    SetBodyPartTissueLayering((BodyPart?)limb ?? organ!);
                }
            }

            var fingerBodyParts = returnParts.Where(i => i is Limb lim && (lim.LimbType is LimbType.Finger
                || lim.LimbType is LimbType.Toe)).ToList();
            var connectorLimbs = returnParts.Where(i => i.BodyPartFunction is BodyPartFunction.Grasp
                || i.BodyPartFunction is BodyPartFunction.Stance).ToList();

            foreach (Limb smallBp in fingerBodyParts.Cast<Limb>())
            {
                DealWithDigits(returnParts, connectorLimbs, smallBp);
            }

            if (Numbers > 0)
                DealWithMoreThanOnePart(returnParts);

            return returnParts;
        }

        private void SetBodyPartTissueLayering(BodyPart bp)
        {
            TissueLayeringTemplate? tisLayer = default;
            string[]? finalList = null;
            // this is bad!
            for (int i = 0; i < _raceTissueLayering.Count; i++)
            {
                tisLayer = _raceTissueLayering[i];
                switch (tisLayer.Select)
                {
                    case SelectContext.LimbType:
                        if (bp is not Limb limb)
                            continue;

                        finalList = Array.FindAll(tisLayer.BodyParts,
                            i => i.Contains(limb.LimbType.ToString()));
                        break;

                    case SelectContext.OrganType:
                        if (bp is not Organ organ)
                            continue;
                        finalList = Array.FindAll(tisLayer.BodyParts,
                            i => i.Contains(organ.OrganType.ToString()));
                        break;

                    case SelectContext.BodyPartFunction:
                        finalList = Array.FindAll(tisLayer.BodyParts,
                            i => i.Contains(bp.ToString()!));
                        break;

                    case SelectContext.Id:
                        finalList = Array.FindAll(tisLayer.BodyParts,
                            i => i.Contains(bp.Id));
                        break;

                    case SelectContext.Category:
                        finalList = Array.FindAll(tisLayer.BodyParts,
                            i => !string.IsNullOrEmpty(bp.Category) && i.Contains(bp?.Category!));
                        break;

                    default:
                        Locator.GetService<MagiLog>().Log("No select context found!");
                        throw new ApplicationException("No select context found!");
                }
                if (finalList.Length > 0)
                    break;
            }
            if (finalList is null)
            {
                Locator.GetService<MagiLog>().Log("No final list for layering tissue found!");
                throw new ApplicationException("No final list for layering tissue found!");
            }

            for (int i = 0; i < finalList.Length; i++)
            {
                var layer = finalList[i].Split(':');
                if (layer.Length == 0)
                    continue;
                foreach (var tissueId in tisLayer!.Tissues)
                {
                    var tissue = _raceTissue[tissueId];
                    if (layer.Length > 1)
                    {
                        for (int z = 1; z < layer.Length; z++)
                        {
                            if (int.TryParse(layer[z], out int thicc))
                                tissue.RelativeThickness = thicc;
                        }
                    }
                    bp.Tissues.Add(tissue);
                }
            }
        }

        private void DealWithMoreThanOnePart(List<BodyPart> returnParts)
        {
            int originalCount = returnParts.Count;
            int copiesToAdd = Numbers * originalCount;
            for (int i = 0; i < copiesToAdd; i++)
            {
                int originalIndex = i / Numbers;
                BodyPart originalPart = returnParts[originalIndex];
                BodyPart copy = originalPart.Copy();
                copy.BodyPartName = string.Format(copy.BodyPartName, i + 1);
                returnParts.Add(copy);
            }
            returnParts.RemoveRange(0, originalCount);
        }

        private static void DealWithDigits(List<BodyPart> returnParts,
            List<BodyPart> connectorLimbs, Limb smallBp)
        {
            returnParts.Remove(smallBp);
            foreach (Limb limb in connectorLimbs.Cast<Limb>())
            {
                if (smallBp.LimbType is LimbType.Finger && limb.BodyPartFunction is BodyPartFunction.Grasp)
                {
                    CreateNewDigitAndAdd(returnParts, smallBp, limb);
                }
                if (smallBp.LimbType is LimbType.Toe && limb.BodyPartFunction is BodyPartFunction.Stance)
                {
                    CreateNewDigitAndAdd(returnParts, smallBp, limb);
                }
            }
        }

        private static void CreateNewDigitAndAdd(List<BodyPart> returnParts, Limb smallBp, Limb limb)
        {
            var template = smallBp;
            Limb digit = template.Copy();
            digit.Orientation = limb.Orientation;
            string orientation = digit.Orientation.ToString().ToLower();
            digit.Id = $"{digit.Id}_{orientation}";
            digit.ConnectedTo = limb.Id;
            digit.BodyPartName = digit.BodyPartName.Replace("{0}", $"{digit.Orientation}");
            returnParts.Add(digit);
        }
    }
}
