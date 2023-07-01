using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.Data.Serialization.EntitySerialization
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
        private readonly List<Tissue> raceTissue = new();
        private readonly List<TissueLayeringTemplate> raceTissueLayering = new();

        public string Id { get; set; }
        public List<string> BodyParts { get; set; }

        //public List<Tissue> Tissues { get; set; }
        public int Numbers { get; set; }

        public BodyPlan()
        {
            BodyParts = new();
        }

        public List<BodyPart> ReturnBodyParts(Race race = null!)
        {
            List<BodyPart> returnParts = new List<BodyPart>();
            string[] tissues = race?.Tissues;
            if (tissues?.Length > 0)
            {
                for (int i = 0; i < tissues.Length; i++)
                {
                    var tissueGroup = DataManager.QueryTissuePlanInData(tissues[0]);
                    if (tissueGroup.Tissues is not null)
                    {
                        raceTissue.AddRange(tissueGroup.Tissues);
                    }
                    if (tissueGroup.TissueLayering is not null)
                        raceTissueLayering.AddRange(tissueGroup.TissueLayering);
                }
            }

            foreach (string bp in BodyParts)
            {
                Limb limb = DataManager.QueryLimbInData(bp);
                Organ organ = DataManager.QueryOrganInData(bp);
                if (limb is not null)
                {
                    returnParts.Add(limb);
                    if (raceTissueLayering.Count > 0 && raceTissue.Count > 0)
                    {
                        for (int i = 0; i < tissues.Length; i++)
                        {
                            var tissueStr = tissues[i].Split(";");
                            if (tissueStr.Length >= 3)
                            {
                                var tissue = new Tissue(tissueStr[0], tissueStr[1], int.Parse(tissueStr[2]));
                                if (tissueStr.Length > 3)
                                {
                                    var flags = tissueStr[3].Split(",");

                                    for (int f = 0; f < flags.Length; f++)
                                    {
                                        if (Enum.TryParse<TissueFlag>(flags[f].Trim(), true, out var res))
                                            tissue.Flags.Add(res);
                                    }
                                }
                                limb.Tissues.Add(tissue);
                            }
                        }
                    }
                }
                if (organ is not null)
                {
                    returnParts.Add(organ);
                }
                if (limb is null && organ is null)
                    throw new ApplicationException($"Coudn't find a valid body part! bodypart id: {bp}");
            }

            for (int i = 0; i < raceTissueLayering.Count; i++)
            {
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