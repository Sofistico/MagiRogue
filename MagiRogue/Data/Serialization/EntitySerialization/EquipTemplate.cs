namespace MagiRogue.Data.Serialization.EntitySerialization
{
    public class EquipTemplate
    {
        public ItemTemplate ItemEquipped { get; set; }
        public LimbTemplate LimbEquipped { get; set; }

        public EquipTemplate(ItemTemplate itemEquipped, LimbTemplate limbEquipped)
        {
            ItemEquipped = itemEquipped;
            LimbEquipped = limbEquipped;
        }
    }
}