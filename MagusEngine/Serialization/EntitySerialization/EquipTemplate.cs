namespace MagusEngine.Serialization.EntitySerialization
{
    public class EquipTemplate
    {
        public string ItemEquipped { get; set; }
        public string LimbEquipped { get; set; }

        public EquipTemplate(string itemEquipped, string limbEquipped)
        {
            ItemEquipped = itemEquipped;
            LimbEquipped = limbEquipped;
        }
    }
}