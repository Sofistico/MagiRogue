namespace MagusEngine.Core
{
    public class DamageType
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;

        /// <summary>
        /// Should be a string with maximum 3 phrases
        /// </summary>
        public string[] SeverityDmgString { get; set; } = null!;
        public string[]? CombinatedFrom { get; set; }
        public bool Magical { get; set; }

        public DamageType()
        {
        }

        public DamageType(string id, string name, string[] severityDmgString, string[]? combinatedFrom, bool magical)
        {
            Id = id;
            Name = name;
            SeverityDmgString = severityDmgString;
            CombinatedFrom = combinatedFrom;
            Magical = magical;
        }
    }
}
