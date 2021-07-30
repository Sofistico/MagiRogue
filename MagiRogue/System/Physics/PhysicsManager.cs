using MagiRogue.Data;
using MagiRogue.Data.Materials;
using System.Linq;

namespace MagiRogue.System.Physics
{
    // For now it just deals with the list material, but in the future all interactions will go here
    public class PhysicsManager
    {
        protected PhysicsManager()
        {
        }

        /// <summary>
        /// Sets the material of the object by the id of the material
        /// </summary>
        /// <param name="id">Id of the material you want, must consult the json file</param>
        /// <returns></returns>
        public static Material SetMaterial(string id) => DataManager.ListOfMaterials.FirstOrDefault(a => a.Id == id);
    }
}