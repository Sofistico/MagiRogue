using Arquimedes.Enumerators;

namespace MagusEngine.Core.Entities.Base
{
    public class Profession
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public AbilityName[] Ability { get; set; }
    }
}