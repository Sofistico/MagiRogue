using MagiRogue.Data.Enumerators;
using SadRogue.Primitives;

namespace MagiRogue.GameSys.Tiles
{
    public sealed class TileDoor : TileBase
    {
        public bool Locked; // Locked door = 1, Unlocked = 0
        public bool IsOpen; // Open door = 1, closed = 0

        /// <summary>
        /// Default constructor
        /// \nA TileDoor can be set locked/unlocked/open/closed using the constructor.
        /// </summary>
        /// <param name="locked">If the door is locked</param>
        /// <param name="open">If the door is open</param>
        /// <param name="position">The position in the map of the door</param>
        /// <param name="idMaterial">The id of the material that will be used to make the door</param>
        public TileDoor(string name, bool locked, bool open, Point position, string idMaterial)
            : base(Color.Gray, Color.Transparent, '+', (int)MapLayer.TERRAIN, position, isTransparent: false, idOfMaterial: idMaterial)
        {
            Name = name;

            //+ is the closed glyph
            //closed by default
            Glyph = '+';

            //Update door fields
            Locked = locked;
            IsOpen = open;

            //change the symbol to open if the door is open
            if (!Locked && IsOpen)
                Open();
            else if (Locked || !IsOpen)
                Close();
        }

        //closes a door
        public void Close()
        {
            IsOpen = false;
            IsBlockingMove = true;
            IsTransparent = false;
            Glyph = '+';
            LastSeenAppereance.Glyph = Glyph;
        }

        // opens a door
        public void Open()
        {
            IsOpen = true;
            IsTransparent = true;
            IsBlockingMove = false;
            Glyph = '-';
            LastSeenAppereance.Glyph = Glyph;
        }
    }
}