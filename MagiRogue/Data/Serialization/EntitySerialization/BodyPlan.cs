using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Serialization.EntitySerialization
{
    public class BodyPlan
    {
        public string Id { get; set; }
        public List<string> BodyParts { get; set; }

        public List<BodyPart> ReturnBodyParts()
        {
            List<BodyPart> returnParts = new List<BodyPart>();

            foreach (string bp in BodyParts)
            {
                Limb limb = DataManager.QueryLimbInData(bp);
                Organ organ = DataManager.QueryOrganInData(bp);
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
            }

            return returnParts;
        }
    }
}