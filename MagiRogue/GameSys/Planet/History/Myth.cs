using MagiRogue.Data.Enumerators;

namespace MagiRogue.GameSys.Planet.History
{
    public class Myth
    {
        public int Id { get; set; }
        public MythWho MythWho { get; set; }
        public MythAction MythAction { get; set; }
        public MythWhat MythWhat { get; set; }
        public string[] Whos { get; set; }
        public string[] WhosIsAffected { get; set; }

        public Myth(int id)
        {
            Id = id;
        }

        public Myth(int id, MythWho mythWho, MythAction action, MythWhat whatAction)
        {
            Id = id;
            MythWho = mythWho;
            MythAction = action;
            MythWhat = whatAction;
        }
    }
}