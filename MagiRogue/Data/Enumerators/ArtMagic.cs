using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    /// <summary>
    /// The arts of magic
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ArtMagic
    {
        ///<summary>projects mana into different forms, mainly made for combat magic</summary>
        Projection,
        ///<summary>The art of unmaking spells, of dispelling and countering magic</summary>
        Negation,
        ///<summary>The art of animating something with magic, making it do someting autonomosly</summary>
        Animation,
        ///<summary>The art of knowing things with magic</summary>
        Divination,
        ///<summary>The art of natural and unnatural trasformation of things with magic</summary>
        Alteration, // takes care of the old Transformation as well.
        ///<summary>The art of protecting something with magic, ex. Ward against fire or metal</summary>
        Wards,
        ///<summary>The art of alterating dimensions with magic, like pocket dimensions and bags of holding</summary>
        Dimensionalism,
        ///<summary>The art of conjuring things with magic</summary>
        Conjuration,
        ///<summary>The art of ilusion, of creating light, of affecting the senses</summary>
        Illuminism,
        ///<summary>The art of mind reading, of implating thought and deleting memories</summary>
        MindMagic,
        ///<summary>The art of the soul, of splicing souls, creating undead and becoming more</summary>
        SoulMagic,
        ///<summary>The art of the body, of sacrifice, of giving oneself or the self of others,
        ///of making permanent arcane alterations to the body and sacrificial power,
        ///of healing as well.</summary>
        BloodMagic
    }
}