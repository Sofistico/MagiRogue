using MagiRogue.Data;
using MagiRogue.Data.Serialization;
using MagiRogue.Utils;

namespace MagiRogue.GameSys.Physics
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
        public static MaterialTemplate SetMaterial(string id) =>
            DataManager.QueryMaterials(id);

        public static int CalculateStrikeForce(float weight, int actorStrikeForce)
        {
            return (int)MathMagi.Round(weight * actorStrikeForce);
        }
    }
}