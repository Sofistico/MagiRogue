using MagiRogue.Data.Materials;
using MagiRogue.System.Magic;
using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MagiRogue.Data
{
    public static class DataManager
    {
        private readonly static string _appDomain = AppDomain.CurrentDomain.BaseDirectory;

        public readonly static IReadOnlyList<ItemTemplate> ListOfItems =
            JsonUtils.JsonDeseralize<List<ItemTemplate>>(Path.Combine(
                _appDomain,
                "Data", "Items", "Bars.json"));

        public readonly static IReadOnlyList<MaterialTemplate> ListOfMaterials =
            JsonUtils.JsonDeseralize<List<MaterialTemplate>>(Path.Combine
        (_appDomain, "Data", "Materials", "MaterialDefinition.json"));

        public readonly static IReadOnlyList<SpellBase> ListOfProjectionSpells =
            JsonUtils.JsonDeseralize<IReadOnlyList<SpellBase>>
            (Path.Combine(_appDomain, "Data", "Spells", "ProjectionSpells.json"));

        public static SpellBase QuerySpellInData(string spellId) => (SpellBase)ListOfProjectionSpells.FirstOrDefault
                (m => m.SpellId.Equals(spellId));
    }
}