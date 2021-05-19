using MagiRogue.Entities.Materials;
using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MagiRogue.System.Physics
{
    // For now it just deals with the list material, but in the future all interactions will go here
    public class PhysicsManager
    {
        public static IEnumerable<Material> ListOfMaterials = JsonUtils.JsonDeseralize<List<Material>>(Path.Combine
                (AppDomain.CurrentDomain.BaseDirectory.ToString(), "Entities", "Materials", "MaterialDefinition.json"));

        public PhysicsManager()
        {
        }

        /// <summary>
        /// Sets the material of the object by the id of the material
        /// </summary>
        /// <param name="id">Id of the material you want, must consult the json file</param>
        /// <returns></returns>
        public static Material SetMaterial(string id) => ListOfMaterials.FirstOrDefault(a => a.Id == id);
    }
}