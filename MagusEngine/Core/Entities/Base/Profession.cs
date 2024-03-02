using Arquimedes.Enumerators;

namespace MagusEngine.Core.Entities.Base
{
    public class Profession : IJsonKey
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public AbilityCategory[] Ability { get; set; }
    }
}