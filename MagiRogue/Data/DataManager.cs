using MagiRogue.Data.Materials;
using MagiRogue.Entities;
using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace MagiRogue.Data
{
    public class DataManager
    {
        public readonly static List<ItemTemplate> ListOfItems =
            JsonUtils.JsonDeseralize<List<ItemTemplate>>(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data", "Items", "Bars.json"));

        public readonly static List<Material> ListOfMaterials =
            JsonUtils.JsonDeseralize<List<Material>>(Path.Combine
        (AppDomain.CurrentDomain.BaseDirectory.ToString(), "Data", "Materials", "MaterialDefinition.json"));
    }
}