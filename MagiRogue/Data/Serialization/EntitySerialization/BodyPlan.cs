using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.Data.Serialization.EntitySerialization
{
    public class BodyPlan
    {
        public string Id { get; set; }
        public List<string> BodyParts { get; set; }
        //public List<Tissue> Tissues { get; set; }

        public BodyPlan()
        {
            BodyParts = new();
        }

        public List<BodyPart> ReturnBodyParts(Race race = null!)
        {
            List<BodyPart> returnParts = new List<BodyPart>();
            string[] tissues = race?.Tissues;

            foreach (string bp in BodyParts)
            {
                Limb limb = DataManager.QueryLimbInData(bp);
                Organ organ = DataManager.QueryOrganInData(bp);
                if (limb is not null)
                {
                    returnParts.Add(limb);
                    if (tissues?.Length > 0)
                    {
                        for (int i = 0; i < tissues.Length; i++)
                        {
                            var str = tissues[i].Split(";");
                            if (str.Length >= 3)
                            {
                                var tissue = new Tissue(str[0], str[1], int.Parse(str[2]));
                                if (str.Length > 3)
                                {
                                    var flags = str[3].Split(",");

                                    for (int f = 0; f < flags.Length; f++)
                                    {
                                        if (Enum.TryParse<TissueFlag>(flags[f].Trim(), true, out var res))
                                            tissue.Flags.Add(res);
                                    }
                                }
                                limb.Tissues.Add(tissue);
                            }
                            else
                            {
                                GameLoop.WriteToLog($"Something went wrong in creating the tissue for BP {limb.Id} and Race {race.Id}");
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

            var bodyParts = returnParts.Where(i => i is Limb lim && (lim.LimbType is LimbType.Finger
                || lim.LimbType is LimbType.Toe)).ToList();
            var connectorLimbs = returnParts.Where(i => i.BodyPartFunction is BodyPartFunction.Grasp
                || i.BodyPartFunction is BodyPartFunction.Stance).ToList();

            foreach (Limb smallBp in bodyParts.Cast<Limb>())
            {
                DealWithDigits(returnParts, connectorLimbs, smallBp);
            }
            return returnParts;
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