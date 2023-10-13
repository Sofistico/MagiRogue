namespace Arquimedes.Enumerators
{
    // If it stops working, add back the player map layer
    /// <summary>
    /// enum for defining maplayer for things, so that a monster and a player can occupy the same tile as
    /// an item for example.
    /// </summary>
    public enum MapLayer
    {
        TERRAIN, // does not support more than one
        //VEGETATION, // supports more than one
        GHOSTS, // supports more than one
        ITEMS, // supports more than one
        FURNITURE, // supports more than one
        ACTORS, // supports more than one
        SPECIAL, // supports more than one, should be always the last layer!
    }
}