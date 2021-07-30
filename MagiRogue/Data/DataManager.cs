using MagiRogue.Data.Materials;
using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace MagiRogue.Data
{
    public class DataManager
    {
        public readonly static IReadOnlyList<ItemTemplate> ListOfItems =
            JsonUtils.JsonDeseralize<List<ItemTemplate>>(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data", "Items", "Bars.json"));

        public readonly static IReadOnlyList<MaterialTemplate> ListOfMaterials =
            JsonUtils.JsonDeseralize<List<MaterialTemplate>>(Path.Combine
        (AppDomain.CurrentDomain.BaseDirectory.ToString(), "Data", "Materials", "MaterialDefinition.json"));
    }
}