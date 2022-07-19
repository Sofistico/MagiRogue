namespace MagiRogue.Data.Enumerators
{
    // If it stops working, add back the player map layer
    /// <summary>
    /// enum for defining maplayer for things, so that a monster and a player can occupy the same tile as
    /// an item for example.
    /// </summary>
    public enum MapLayer
    {
        TERRAIN,
        GHOSTS,
        ITEMS,
        ACTORS,
        FURNITURE,
        PLAYER
    }
}