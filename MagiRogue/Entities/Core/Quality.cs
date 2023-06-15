using MagiRogue.Data.Enumerators;
using System;
using System.Collections.Generic;

namespace MagiRogue.Entities.Core
{
    /// <summary>
    /// Is anything that can be interacted with.
    /// </summary>
    public struct Quality
    {
        public QualityType QualityType { get; set; }
        public int QualitySuitabiliy { get; set; }

        public Quality(QualityType qualityType, int qualitySuitabiliy)
        {
            QualityType = qualityType;
            QualitySuitabiliy = qualitySuitabiliy;
        }

        public static List<Quality> ReturnQualityList(List<List<string>> qualities)
        {
            if (qualities is null)
                return new List<Quality>();
            List<Quality> ret = new List<Quality>();
            try
            {
                foreach (List<string> list in qualities)
                {
                    QualityType quality = Enum.Parse<QualityType>(list[0]);
                    int qualityInt = int.Parse(list[1]);
                    Quality item = new Quality(quality, qualityInt);
                    ret.Add(item);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Tried to add a quality with error! Message: {ex.Message}");
            }
            return ret;
        }

        public static List<List<string>> ReturnQualityListAsString(List<Quality> list)
        {
            List<List<string>> ret = new List<List<string>>();

            foreach (Quality item in list)
            {
                string[] qualityString = item.ToString().Split("-");
                ret.Add(new List<string> { qualityString[0], qualityString[1] });
            }

            return ret;
        }

        public override string ToString()
        {
            return $"{QualityType}-{QualitySuitabiliy}";
        }
    }
}